using CourseManagementSystem.Models;
using CourseManagementSystem.Repository;
using CourseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CourseManagementSystem.Controllers
{
    public class LessonController : Controller
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] AllowedVideoExts       = [".mp4", ".webm", ".mov", ".mkv"];
        private static readonly string[] AllowedAttachmentExts  = [".pdf", ".docx", ".pptx", ".zip"];

        public LessonController(ILessonRepository lessonRepository, ApplicationDbContext context, IWebHostEnvironment env)
        {
            _lessonRepository = lessonRepository;
            _context          = context;
            _env              = env;
        }

        private bool OwnsParentSection(int sectionId)
        {
            if (User.IsInRole("Admin")) return true;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return _context.Sections
                .Include(s => s.Course)
                .Any(s => s.Id == sectionId && s.Course!.InstructorId == userId);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index(int sectionId)
        {
            ViewBag.SectionId = sectionId;
            var lessons = _lessonRepository.GetLessonsBySectionId(sectionId);
            return View(lessons);
        }

        [HttpGet]
        [Authorize(Roles = "Instructor,Admin")]
        public IActionResult Create(int sectionId)
        {
            if (!OwnsParentSection(sectionId)) return Forbid();
            return View(new LessonViewModel { SectionId = sectionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> Create(LessonViewModel vm)
        {
            if (!OwnsParentSection(vm.SectionId)) return Forbid();

            ModelState.Remove("VideoUrl");
            if (!ModelState.IsValid) return View(vm);

            var lesson = new Lesson
            {
                Title           = vm.Title,
                SectionId       = vm.SectionId,
                IsFreePreview   = vm.IsFreePreview,
                DurationMinutes = vm.DurationMinutes
            };

            ApplyVideo(vm, lesson, null);

            _lessonRepository.Add(lesson);
            _lessonRepository.Save();

            await SaveAttachmentsAsync(vm.AttachmentFiles, lesson.Id);

            TempData["Success"] = "Lesson created.";
            return RedirectToAction(nameof(Index), new { sectionId = vm.SectionId });
        }

        [HttpGet]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Attachments)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null) return NotFound();
            if (!OwnsParentSection(lesson.SectionId)) return Forbid();

            var vm = new LessonViewModel
            {
                Id                    = lesson.Id,
                Title                 = lesson.Title,
                SectionId             = lesson.SectionId,
                VideoType             = lesson.VideoType,
                VideoUrl              = lesson.VideoUrl,
                ExistingVideoFileName = lesson.UploadedVideoFileName,
                DurationMinutes       = lesson.DurationMinutes,
                IsFreePreview         = lesson.IsFreePreview,
                ExistingAttachments   = lesson.Attachments?.Select(a => new AttachmentVM
                {
                    Id       = a.Id,
                    FileName = a.FileName,
                    FileType = a.FileType,
                    FileSize = a.FileSize
                }).ToList() ?? new()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> Edit(int id, LessonViewModel vm)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Attachments)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null) return NotFound();
            if (!OwnsParentSection(lesson.SectionId)) return Forbid();

            ModelState.Remove("VideoUrl");
            if (!ModelState.IsValid)
            {
                vm.ExistingAttachments = lesson.Attachments?.Select(a => new AttachmentVM
                {
                    Id = a.Id, FileName = a.FileName, FileType = a.FileType, FileSize = a.FileSize
                }).ToList() ?? new();
                return View(vm);
            }

            lesson.Title           = vm.Title;
            lesson.IsFreePreview   = vm.IsFreePreview;
            lesson.DurationMinutes = vm.DurationMinutes;

            ApplyVideo(vm, lesson, lesson.UploadedVideoFileName);

            _lessonRepository.Update(lesson);
            _lessonRepository.Save();

            await SaveAttachmentsAsync(vm.AttachmentFiles, lesson.Id);

            TempData["Success"] = "Lesson updated.";
            return RedirectToAction(nameof(Index), new { sectionId = lesson.SectionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> DeleteAttachment(int attachmentId, int lessonId)
        {
            var attachment = await _context.Attachments.FindAsync(attachmentId);
            if (attachment != null)
            {
                DeleteFile(Path.Combine("uploads", "attachments", attachment.StoredFileName));
                _context.Attachments.Remove(attachment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Attachment deleted.";
            }
            return RedirectToAction(nameof(Edit), new { id = lessonId });
        }

        [HttpGet]
        [Authorize(Roles = "Instructor,Admin")]
        public IActionResult Delete(int id)
        {
            var lesson = _lessonRepository.GetById(id);
            if (lesson == null) return NotFound();
            if (!OwnsParentSection(lesson.SectionId)) return Forbid();
            return View(lesson);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> Delete(int id, Lesson lessonFromForm)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Attachments)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null) return NotFound();
            if (!OwnsParentSection(lesson.SectionId)) return Forbid();

            if (!string.IsNullOrEmpty(lesson.UploadedVideoFileName))
                DeleteFile(Path.Combine("uploads", "videos", lesson.UploadedVideoFileName));

            foreach (var att in lesson.Attachments ?? [])
                DeleteFile(Path.Combine("uploads", "attachments", att.StoredFileName));

            int sectionId = lesson.SectionId;
            _lessonRepository.Delete(id);
            _lessonRepository.Save();

            TempData["Success"] = "Lesson deleted.";
            return RedirectToAction(nameof(Index), new { sectionId });
        }

        // ── Helpers ────────────────────────────────────────────────────

        private void ApplyVideo(LessonViewModel vm, Lesson lesson, string? oldVideoFile)
        {
            if (vm.VideoType == "youtube" && !string.IsNullOrEmpty(vm.VideoUrl))
            {
                if (!string.IsNullOrEmpty(oldVideoFile))
                    DeleteFile(Path.Combine("uploads", "videos", oldVideoFile));
                lesson.VideoType             = "youtube";
                lesson.VideoUrl              = vm.VideoUrl;
                lesson.UploadedVideoFileName = null;
            }
            else if (vm.VideoType == "upload" && vm.VideoFile is { Length: > 0 })
            {
                var ext = Path.GetExtension(vm.VideoFile.FileName).ToLowerInvariant();
                if (AllowedVideoExts.Contains(ext))
                {
                    if (!string.IsNullOrEmpty(oldVideoFile))
                        DeleteFile(Path.Combine("uploads", "videos", oldVideoFile));
                    var stored = $"{Guid.NewGuid()}{ext}";
                    SaveFile(vm.VideoFile, Path.Combine("uploads", "videos", stored));
                    lesson.VideoType             = "upload";
                    lesson.VideoUrl              = null;
                    lesson.UploadedVideoFileName = stored;
                }
            }
            else if (vm.VideoType == "none" || string.IsNullOrEmpty(vm.VideoType))
            {
                if (!string.IsNullOrEmpty(oldVideoFile))
                    DeleteFile(Path.Combine("uploads", "videos", oldVideoFile));
                lesson.VideoType             = null;
                lesson.VideoUrl              = null;
                lesson.UploadedVideoFileName = null;
            }
        }

        private async Task SaveAttachmentsAsync(List<IFormFile> files, int lessonId)
        {
            foreach (var file in files.Where(f => f.Length > 0))
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant().TrimStart('.');
                if (!AllowedAttachmentExts.Contains("." + ext)) continue;

                var stored = $"{Guid.NewGuid()}.{ext}";
                SaveFile(file, Path.Combine("uploads", "attachments", stored));

                _context.Attachments.Add(new Attachment
                {
                    LessonId       = lessonId,
                    FileName       = Path.GetFileName(file.FileName),
                    StoredFileName = stored,
                    FileType       = ext,
                    FileSize       = file.Length,
                    UploadedAt     = DateTime.UtcNow
                });
            }
            await _context.SaveChangesAsync();
        }

        private void SaveFile(IFormFile file, string relativePath)
        {
            var fullPath = Path.Combine(_env.WebRootPath, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            using var stream = new FileStream(fullPath, FileMode.Create);
            file.CopyTo(stream);
        }

        private void DeleteFile(string relativePath)
        {
            var fullPath = Path.Combine(_env.WebRootPath, relativePath);
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
    }
}
