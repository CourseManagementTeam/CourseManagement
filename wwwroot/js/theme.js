const themeBtn = document.getElementById("themeBtn");

if(localStorage.getItem("theme"))
{
    document.documentElement.setAttribute(
        "data-bs-theme",
        localStorage.getItem("theme")
    );
}

themeBtn?.addEventListener("click",()=>{

    let current =
    document.documentElement.getAttribute("data-bs-theme");

    let next =
    current === "light" ? "dark" : "light";

    document.documentElement.setAttribute(
        "data-bs-theme",
        next
    );

    localStorage.setItem(
        "theme",
        next
    );
});