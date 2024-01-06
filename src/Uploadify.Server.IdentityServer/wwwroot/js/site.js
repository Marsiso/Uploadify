document.addEventListener('DOMContentLoaded', function () {
    const inputFields = document.querySelectorAll('input.validation-error');

    inputFields.forEach(function (input) {
        input.addEventListener('change', function () {
            const label = this.closest('label');
            if (label && label.classList.contains('validation-error')) {
                label.classList.remove('validation-error');
            }

            const error = this.closest('div').querySelector('.help-text');
            if (error) {
                error.remove();
            }

            input.classList.remove('validation-error');
        });
    });
});
