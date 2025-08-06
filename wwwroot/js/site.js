function toggleSearch() {
  const searchBar = document.getElementById("searchBar");
  searchBar.classList.toggle("active");

  if (searchBar.classList.contains("active")) {
    const input = searchBar.querySelector('input[type="text"]');
    if (input) setTimeout(() => input.focus(), 100);
  }
}

// Ẩn thanh tìm kiếm khi click ra ngoài
document.addEventListener("click", function (e) {
  const searchBar = document.getElementById("searchBar");
  const searchToggle = document.querySelector(".search-toggle");

  if (
    searchBar.classList.contains("active") &&
    !searchBar.contains(e.target) &&
    !searchToggle.contains(e.target)
  ) {
    searchBar.classList.remove("active");
  }
});
