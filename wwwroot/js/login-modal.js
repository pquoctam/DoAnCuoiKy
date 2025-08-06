function toggleLoginModal() {
  const modal = document.getElementById("loginModal");
  if (modal) {
    modal.style.display = (modal.style.display === "flex") ? "none" : "flex";
  }
}

window.addEventListener('click', function(e) {
  const modal = document.getElementById("loginModal");
  if (modal && e.target === modal) {
    modal.style.display = "none";
  }
});
