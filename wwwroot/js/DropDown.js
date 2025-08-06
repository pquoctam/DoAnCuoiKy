function toggleDropdown() {
  const menu = document.getElementById("dropdownMenu");
  if (menu) {
    menu.classList.toggle("show");
  }
}

// Xử lý ẩn dropdown khi click ra ngoài
document.addEventListener("click", function (event) {
  const account = document.querySelector(".account-dropdown");
  const menu = document.getElementById("dropdownMenu");
  if (account && !account.contains(event.target)) {
    if (menu) menu.classList.remove("show");
  }
});
