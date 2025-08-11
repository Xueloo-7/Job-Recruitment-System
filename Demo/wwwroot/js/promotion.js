document.addEventListener('DOMContentLoaded', () => {
    let selectedId = null;
    const nextBtn = document.getElementById('next-btn');
    const selectBtns = document.querySelectorAll('.select-btn');
    const promoCards = document.querySelectorAll('.promo-card');

    selectBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            // remove active state from all cards and buttons
            promoCards.forEach(card => card.classList.remove('selected'));
            selectBtns.forEach(b => {
                b.textContent = 'Apply';
                b.classList.remove('selected-btn');
            });

            // set selected state for the clicked card and button
            const card = btn.closest('.promo-card');
            card.classList.add('selected');
            btn.textContent = 'Selected';
            btn.classList.add('selected-btn');

            selectedId = card.dataset.id;

            // avate next button
            nextBtn.disabled = false;
            nextBtn.classList.add('enabled');
        });
    });

    nextBtn.addEventListener('click', () => {
        if (selectedId) {
            window.location.href = `/Home/Index?promotionId=${selectedId}`;
        }
    });

    // disable next button initially
    nextBtn.classList.remove('enabled');
    nextBtn.disabled = true;
});