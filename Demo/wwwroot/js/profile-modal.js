// 通用确认弹窗
(function () {
    const modal = document.getElementById('confirmModal');
    const title = document.getElementById('confirmModalTitle');
    const content = document.getElementById('confirmModalContent');
    const btnOk = document.getElementById('confirmOk');
    const btnCancel = document.getElementById('confirmCancel');

    let confirmCallback = null;

    window.showConfirmModal = function (options) {
        // options: {title?, message, onConfirm}
        title.textContent = options.title || '确认操作';
        content.textContent = options.message || '确定要执行此操作吗？';
        confirmCallback = options.onConfirm;

        modal.classList.add('active');
        document.body.style.overflow = 'hidden';
    };

    function closeModal() {
        modal.classList.remove('active');
        document.body.style.overflow = '';
    }

    btnOk.addEventListener('click', function () {
        if (typeof confirmCallback === 'function') confirmCallback();
        closeModal();
    });
    btnCancel.addEventListener('click', closeModal);
})();
