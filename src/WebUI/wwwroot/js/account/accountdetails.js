function showProfile() {
    document.getElementById("profile").className = "nav-link hotel-hover hotel-active";
    document.getElementById("password").className = "nav-link hotel-hover";
    document.getElementById("profileview").style.display = 'block';
    document.getElementById("passwordview").style.display = 'none';
}

function showPassword() {
    document.getElementById("profile").className = "nav-link hotel-hover";
    document.getElementById("password").className = "nav-link hotel-hover hotel-active";
    document.getElementById("profileview").style.display = 'none';
    document.getElementById("passwordview").style.display = 'block';
}

window.onload = function fadeOut() {
    if (document.getElementById("messageS") !== null)
        setTimeout(function () { $(messageS).fadeOut("slow") }, 4000)
}

document.getElementById("username").addEventListener("input", function () {
    document.getElementById("usernamewarning").style.display = 'block';
});