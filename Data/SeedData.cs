using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Data
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var db = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // ── 1. Roles ──────────────────────────────────────────────────
            foreach (var role in new[] { "Admin", "Instructor", "Student" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // ── 2. Users ──────────────────────────────────────────────────
            var adminUser = await EnsureUser(userManager,
                email: "admin@cms.com",
                fullName: "Alex Morgan",
                bio: "Platform administrator and learning experience designer.",
                password: "Admin123!",
                role: "Admin");

            var instructorUser = await EnsureUser(userManager,
                email: "instructor@cms.com",
                fullName: "Sarah Johnson",
                bio: "Senior software engineer and full-stack educator with 10 years of industry experience.",
                password: "Instructor123!",
                role: "Instructor");

            var studentUser = await EnsureUser(userManager,
                email: "student@cms.com",
                fullName: "James Wilson",
                bio: "Aspiring developer passionate about building web applications.",
                password: "Student123!",
                role: "Student");

            // ── 3. Categories ─────────────────────────────────────────────
            if (!await db.Categories.AnyAsync())
            {
                db.Categories.AddRange(
                    new Category { Name = "Web Development", Description = "Frontend, backend and full-stack web technologies." },
                    new Category { Name = "Mobile Development", Description = "iOS, Android and cross-platform app development." },
                    new Category { Name = "Data Science", Description = "Data analysis, machine learning and AI." },
                    new Category { Name = "UI/UX Design", Description = "User interface design, prototyping and usability." },
                    new Category { Name = "Cloud & DevOps", Description = "Cloud platforms, CI/CD pipelines and infrastructure." }
                );
                await db.SaveChangesAsync();
            }

            // ── 4. Courses ────────────────────────────────────────────────
            if (!await db.Courses.AnyAsync())
            {
                var catWeb = await db.Categories.FirstAsync(c => c.Name == "Web Development");
                var catMobile = await db.Categories.FirstAsync(c => c.Name == "Mobile Development");
                var catData = await db.Categories.FirstAsync(c => c.Name == "Data Science");
                var catDesign = await db.Categories.FirstAsync(c => c.Name == "UI/UX Design");
                var catCloud = await db.Categories.FirstAsync(c => c.Name == "Cloud & DevOps");
                var iid = instructorUser.Id;

                var courses = new List<Course>
                {
                    new Course
                    {
                        Title = "Complete ASP.NET Core MVC: Build Real-World Web Apps",
                        Description = "Master ASP.NET Core MVC from scratch. You will learn routing, controllers, views, Entity Framework Core, Identity, and deploy a production-ready application by the end of this course.",
                        Price = 89.99m,
                        Level = "Intermediate",
                        IsPublished = true,
                        AverageRating = 4.8,
                        ReviewsCount = 1,
                        ImageUrl = "asp.png",
                        CategoryId = catWeb.Id,
                        InstructorId = iid,
                        CreatedDate = DateTime.Now.AddDays(-60)
                    },
                    new Course
                    {
                        Title = "Modern JavaScript: ES6 to ES2024 Complete Guide",
                        Description = "Go from JavaScript basics to advanced topics including async/await, Promises, modules, destructuring, and the latest ECMAScript features used in modern web development.",
                        Price = 74.99m,
                        Level = "Beginner",
                        IsPublished = true,
                        AverageRating = 4.6,
                        ReviewsCount = 0,
                        ImageUrl = "JavaScript_149.jpg",
                        CategoryId = catWeb.Id,
                        InstructorId = iid,
                        CreatedDate = DateTime.Now.AddDays(-45)
                    },
                    new Course
                    {
                        Title = "React 19 & Redux Toolkit: Complete Frontend Development",
                        Description = "Build professional React applications using hooks, context, Redux Toolkit, React Query, and TypeScript. Includes three full project builds from design to deployment.",
                        Price = 94.99m,
                        Level = "Intermediate",
                        IsPublished = true,
                        AverageRating = 4.9,
                        ReviewsCount = 0,
                        ImageUrl = "React JS Complete Guide.png",
                        CategoryId = catWeb.Id,
                        InstructorId = iid,
                        CreatedDate = DateTime.Now.AddDays(-30)
                    },
                    new Course
                    {
                        Title = "Flutter & Dart: Build iOS and Android Apps from Scratch",
                        Description = "Learn Flutter development by building five complete mobile applications. Covers state management with Riverpod, REST APIs, Firebase integration, and App Store publishing.",
                        Price = 84.99m,
                        Level = "Beginner",
                        IsPublished = true,
                        AverageRating = 4.7,
                        ReviewsCount = 0,
                        ImageUrl = "flutter.jpeg",
                        CategoryId = catMobile.Id,
                        InstructorId = iid,
                        CreatedDate = DateTime.Now.AddDays(-50)
                    },
                    new Course
                    {
                        Title = "Python for Data Science and Machine Learning Bootcamp",
                        Description = "Master Python for data analysis using NumPy, Pandas, Matplotlib, Scikit-Learn, and TensorFlow. Includes real datasets and hands-on machine learning projects.",
                        Price = 79.99m,
                        Level = "Intermediate",
                        IsPublished = true,
                        AverageRating = 4.8,
                        ReviewsCount = 0,
                        ImageUrl = "labtop_coding.avif",
                        CategoryId = catData.Id,
                        InstructorId = iid,
                        CreatedDate = DateTime.Now.AddDays(-55)
                    },
                    new Course
                    {
                        Title = "UX Design Fundamentals: Research, Wireframes & Prototypes",
                        Description = "Learn user experience design from research through high-fidelity Figma prototypes. Covers user research, information architecture, wireframing, and usability testing.",
                        Price = 69.99m,
                        Level = "Beginner",
                        IsPublished = true,
                        AverageRating = 4.5,
                        ReviewsCount = 0,
                        ImageUrl = "ui&ux.avif",
                        CategoryId = catDesign.Id,
                        InstructorId = iid,
                        CreatedDate = DateTime.Now.AddDays(-40)
                    },
                    new Course
                    {
                        Title = "AWS Solutions Architect: From Zero to Certification",
                        Description = "Comprehensive preparation for the AWS Solutions Architect Associate exam. Covers EC2, S3, RDS, VPC, Lambda, CloudFront and best practice cloud architecture patterns.",
                        Price = 99.99m,
                        Level = "Advanced",
                        IsPublished = true,
                        AverageRating = 4.7,
                        ReviewsCount = 0,
                        ImageUrl = "aws-cloud.jpg",
                        CategoryId = catCloud.Id,
                        InstructorId = iid,
                        CreatedDate = DateTime.Now.AddDays(-20)
                    },
                    new Course
                    {
                        Title = "Docker & Kubernetes: The Practical Container Guide",
                        Description = "Master containerization with Docker and orchestration with Kubernetes. Build, ship and scale applications with CI/CD pipelines using GitHub Actions and Helm charts.",
                        Price = 89.99m,
                        Level = "Intermediate",
                        IsPublished = true,
                        AverageRating = 4.6,
                        ReviewsCount = 0,
                        ImageUrl = "docker-kubernetes.jpg",
                        CategoryId = catCloud.Id,
                        InstructorId = iid,
                        CreatedDate = DateTime.Now.AddDays(-15)
                    }
                };

                db.Courses.AddRange(courses);
                await db.SaveChangesAsync();
            }

            // ── 5. Sections & Lessons ─────────────────────────────────────
            if (!await db.Sections.AnyAsync())
            {
                var allCourses = await db.Courses.ToListAsync();

                var sectionData = new Dictionary<string, List<(string SectionTitle, List<(string Title, string? VideoUrl, bool IsFree)> Lessons)>>
                {
                    ["Complete ASP.NET Core MVC: Build Real-World Web Apps"] = new()
                    {
                        ("Getting Started with ASP.NET Core", new()
                        {
                            ("Introduction & Course Overview", "https://www.youtube.com/watch?v=BfEjDD_30q0", true),
                            ("Setting Up the Development Environment", "https://www.youtube.com/watch?v=BfEjDD_30q0", true),
                            ("Understanding the MVC Pattern", null, false)
                        }),
                        ("Controllers & Routing", new()
                        {
                            ("Creating Your First Controller", null, false),
                            ("Route Parameters and Constraints", null, false),
                            ("Action Results and Responses", null, false)
                        }),
                        ("Entity Framework Core & Database", new()
                        {
                            ("Setting Up DbContext", null, false),
                            ("Code-First Migrations", null, false),
                            ("Repository Pattern Implementation", null, false),
                            ("Seeding Data", null, false)
                        })
                    },
                    ["Modern JavaScript: ES6 to ES2024 Complete Guide"] = new()
                    {
                        ("JavaScript Fundamentals Refresher", new()
                        {
                            ("Variables, Scopes and Hoisting", "https://www.youtube.com/watch?v=BfEjDD_30q0", true),
                            ("Functions and Arrow Functions", "https://www.youtube.com/watch?v=BfEjDD_30q0", true),
                            ("Objects and Prototypes", null, false)
                        }),
                        ("ES6 Core Features", new()
                        {
                            ("Destructuring and Spread Operator", null, false),
                            ("Template Literals and Tagged Templates", null, false),
                            ("Modules: Import and Export", null, false)
                        }),
                        ("Async JavaScript", new()
                        {
                            ("Callbacks and the Event Loop", null, false),
                            ("Promises in Depth", null, false),
                            ("Async/Await Patterns", null, false)
                        })
                    },
                    ["React 19 & Redux Toolkit: Complete Frontend Development"] = new()
                    {
                        ("React Foundations", new()
                        {
                            ("JSX and the Virtual DOM", "https://www.youtube.com/watch?v=BfEjDD_30q0", true),
                            ("Components and Props", "https://www.youtube.com/watch?v=BfEjDD_30q0", true),
                            ("useState and useEffect Hooks", null, false)
                        }),
                        ("State Management with Redux Toolkit", new()
                        {
                            ("Setting Up Redux Store", null, false),
                            ("Slices and Reducers", null, false),
                            ("Async Thunks and RTK Query", null, false)
                        }),
                        ("Building the Project: E-Commerce App", new()
                        {
                            ("Project Architecture and Setup", null, false),
                            ("Product Listing and Cart", null, false),
                            ("Authentication and Protected Routes", null, false),
                            ("Deployment to Vercel", null, false)
                        })
                    },
                    ["Flutter & Dart: Build iOS and Android Apps from Scratch"] = new()
                    {
                        ("Dart Language Essentials", new()
                        {
                            ("Dart Variables and Types", "https://www.youtube.com/watch?v=BfEjDD_30q0", true),
                            ("Classes and Null Safety", null, false),
                            ("Async Programming in Dart", null, false)
                        }),
                        ("Flutter UI Fundamentals", new()
                        {
                            ("Widgets: Stateless vs Stateful", "https://www.youtube.com/watch?v=BfEjDD_30q0", true),
                            ("Layouts: Row, Column, Stack", null, false),
                            ("Navigation and Routing", null, false)
                        }),
                        ("State Management with Riverpod", new()
                        {
                            ("Introduction to Riverpod", null, false),
                            ("Providers and Consumers", null, false),
                            ("AsyncNotifier and FutureProvider", null, false)
                        })
                    },
                    ["Python for Data Science and Machine Learning Bootcamp"] = new()
                    {
                        ("Python and NumPy Foundations", new()
                        {
                            ("Python Review for Data Science", "https://www.youtube.com/watch?v=BfEjDD_30q0", true),
                            ("NumPy Arrays and Operations", null, false),
                            ("Broadcasting and Vectorization", null, false)
                        }),
                        ("Data Analysis with Pandas", new()
                        {
                            ("DataFrames and Series", "https://www.youtube.com/watch?v=BfEjDD_30q0", true),
                            ("Data Cleaning and Transformation", null, false),
                            ("GroupBy and Aggregation", null, false),
                            ("Visualization with Matplotlib", null, false)
                        }),
                        ("Machine Learning with Scikit-Learn", new()
                        {
                            ("Supervised vs Unsupervised Learning", null, false),
                            ("Linear and Logistic Regression", null, false),
                            ("Decision Trees and Random Forests", null, false)
                        })
                    },
                    ["UX Design Fundamentals: Research, Wireframes & Prototypes"] = new()
                    {
                        ("UX Research Methods", new()
                        {
                            ("What is UX Design?", "https://www.youtube.com/watch?v=BfEjDD_30q0", true),
                            ("User Interviews and Surveys", null, false),
                            ("Analyzing Research Data", null, false)
                        }),
                        ("Information Architecture & Wireframing", new()
                        {
                            ("Site Maps and User Flows", "https://www.youtube.com/watch?v=BfEjDD_30q0", true),
                            ("Low-Fidelity Wireframes", null, false),
                            ("Wireframe Best Practices", null, false)
                        }),
                        ("Figma Prototyping", new()
                        {
                            ("Figma Interface Overview", null, false),
                            ("Building High-Fidelity Mockups", null, false),
                            ("Interactive Prototyping and Handoff", null, false)
                        })
                    },
                    ["AWS Solutions Architect: From Zero to Certification"] = new()
                    {
                        ("AWS Core Services", new()
                        {
                            ("IAM: Users, Groups and Policies", "https://www.youtube.com/watch?v=BfEjDD_30q0", true),
                            ("EC2: Instances and AMIs", null, false),
                            ("S3: Storage and Lifecycle Policies", null, false)
                        }),
                        ("Networking and Security", new()
                        {
                            ("VPC, Subnets and Security Groups", "https://www.youtube.com/watch?v=BfEjDD_30q0", true),
                            ("Route 53 and CloudFront", null, false),
                            ("AWS WAF and Shield", null, false)
                        }),
                        ("Databases and High Availability", new()
                        {
                            ("RDS Multi-AZ and Read Replicas", null, false),
                            ("DynamoDB Fundamentals", null, false),
                            ("Elastic Load Balancing and Auto Scaling", null, false)
                        })
                    },
                    ["Docker & Kubernetes: The Practical Container Guide"] = new()
                    {
                        ("Docker Fundamentals", new()
                        {
                            ("What Are Containers?", "https://www.youtube.com/watch?v=BfEjDD_30q0", true),
                            ("Docker Images and Dockerfiles", null, false),
                            ("Docker Compose for Multi-Container Apps", null, false)
                        }),
                        ("Kubernetes Core Concepts", new()
                        {
                            ("Pods, Deployments and Services", "https://www.youtube.com/watch?v=BfEjDD_30q0", true),
                            ("ConfigMaps and Secrets", null, false),
                            ("Persistent Volumes and Storage", null, false)
                        }),
                        ("CI/CD with GitHub Actions and Helm", new()
                        {
                            ("Building a CI Pipeline", null, false),
                            ("Helm Charts and Releases", null, false),
                            ("Deploying to a Managed Kubernetes Cluster", null, false)
                        })
                    }
                };

                int sectionOrder = 1;
                foreach (var course in allCourses)
                {
                    if (!sectionData.TryGetValue(course.Title, out var sections))
                        continue;

                    int secOrder = 1;
                    foreach (var (sectionTitle, lessons) in sections)
                    {
                        var section = new Section
                        {
                            CourseId = course.Id,
                            Title = sectionTitle,
                            OrderNumber = secOrder++
                        };
                        db.Sections.Add(section);
                        await db.SaveChangesAsync();

                        int lesOrder = 1;
                        foreach (var (lesTitle, videoUrl, isFree) in lessons)
                        {
                            db.Lessons.Add(new Lesson
                            {
                                SectionId = section.Id,
                                Title = lesTitle,
                                VideoUrl = videoUrl,
                                IsFreePreview = isFree,
                                OrderNumber = lesOrder++,
                                DurationMinutes = 15 + (lesOrder * 3)
                            });
                        }
                        await db.SaveChangesAsync();
                    }
                }
            }

            // ── 6. Enrollments ────────────────────────────────────────────
            if (!await db.Enrollments.AnyAsync())
            {
                var sid = studentUser.Id;
                var enrollCourses = await db.Courses
                    .OrderBy(c => c.CreatedDate)
                    .Take(3)
                    .ToListAsync();

                foreach (var course in enrollCourses)
                {
                    db.Enrollments.Add(new Enrollment
                    {
                        StudentId = sid,
                        CourseId = course.Id,
                        EnrolledAt = DateTime.Now.AddDays(-new Random().Next(5, 30)),
                        ProgressPercentage = new Random().Next(10, 85),
                        IsCompleted = false
                    });
                }
                await db.SaveChangesAsync();
            }

            // ── 7. Reviews ────────────────────────────────────────────────
            if (!await db.Reviews.AnyAsync())
            {
                var sid = studentUser.Id;
                var enrolledCourseIds = await db.Enrollments
                    .Where(e => e.StudentId == sid)
                    .Select(e => e.CourseId)
                    .ToListAsync();

                if (enrolledCourseIds.Count > 0)
                {
                    var reviewedCourse = await db.Courses
                        .FirstAsync(c => c.Id == enrolledCourseIds[0]);

                    db.Reviews.Add(new Review
                    {
                        CourseId = reviewedCourse.Id,
                        StudentId = sid,
                        Rate = 5,
                        Comment = "Absolutely brilliant course. The instructor explains every concept with real-world examples that actually stick. I went from knowing almost nothing about MVC to building a complete application in just a few weeks. Highly recommended for anyone serious about ASP.NET Core.",
                        CreatedAt = DateTime.Now.AddDays(-5)
                    });

                    // Update the course rating
                    reviewedCourse.AverageRating = 5.0;
                    reviewedCourse.ReviewsCount = 1;
                    db.Courses.Update(reviewedCourse);

                    await db.SaveChangesAsync();
                }
            }

            // ── 8. Wishlist ───────────────────────────────────────────────
            if (!await db.Wishlists.AnyAsync())
            {
                var sid = studentUser.Id;
                var enrolledCourseIds = await db.Enrollments
                    .Where(e => e.StudentId == sid)
                    .Select(e => e.CourseId)
                    .ToListAsync();

                var wishlistCourses = await db.Courses
                    .Where(c => !enrolledCourseIds.Contains(c.Id))
                    .Take(2)
                    .ToListAsync();

                foreach (var course in wishlistCourses)
                {
                    db.Wishlists.Add(new Wishlist
                    {
                        StudentId = sid,
                        CourseId = course.Id,
                        AddedAt = DateTime.Now.AddDays(-new Random().Next(1, 10))
                    });
                }
                await db.SaveChangesAsync();
            }
        }

        private static async Task<ApplicationUser> EnsureUser(
            UserManager<ApplicationUser> userManager,
            string email, string fullName, string bio,
            string password, string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    Bio = bio,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to create seed user {email}: {errors}");
                }
            }
            else
            {
                // Always reset password for seed accounts so credentials stay predictable
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                await userManager.ResetPasswordAsync(user, token, password);
            }

            if (!await userManager.IsInRoleAsync(user, role))
                await userManager.AddToRoleAsync(user, role);

            return user;
        }
    }
}