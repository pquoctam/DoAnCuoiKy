function setupComboSwiper() {
    new Swiper(".comboSwiper", {
      slidesPerView: 2,
      spaceBetween: 30,
      loop: true,
      navigation: {
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev",
      },
      autoplay: {
        delay: 5000,
        disableOnInteraction: false,
      },
      breakpoints: {
        768: { slidesPerView: 2 },
        1024: { slidesPerView: 3 }
      }
    });
  }

  // Gọi hàm để khởi chạy comboSwiper
  setupComboSwiper();