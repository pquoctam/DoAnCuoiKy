function setupNewsSwiper() {
    new Swiper(".newsSwiper", {
      slidesPerView: 2,
      spaceBetween: 30,
      loop: true,
      navigation: {
        nextEl: ".swiper-button-next.news-next",
        prevEl: ".swiper-button-prev.news-prev",
      },
      autoplay: {
        delay: 5000,
        disableOnInteraction: false,
      },
      pagination: {
        el: ".news-pagination",
        clickable: true,
        dynamicBullets: false
      },
      breakpoints: {
        0: { slidesPerView: 1 },
        768: { slidesPerView: 2 }
      }
    });
  }

  // Gọi hàm để khởi tạo khi cần
  setupNewsSwiper();