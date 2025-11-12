// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

/**
 * Displays a Bootstrap 5 Toast notification.
 * @param {string} message The message to display in the toast.
 * @param {'success' | 'danger' | 'warning' | 'info' | 'primary' | 'secondary' | 'light' | 'dark'} [type='info'] The type of toast (determines background color).
 * @param {number} [delay=5000] How long the toast should be visible in milliseconds.
 */
function showToast(message, type = 'info', delay = 5000) {
    const toastContainer = document.querySelector('.toast-container');
    if (!toastContainer) {
        console.error('Toast container not found in the DOM.');
        return;
    }

    const toastId = 'toast-' + Date.now(); // Unique ID for each toast
    const toastHtml = `
        <div id="${toastId}" class="toast align-items-center text-white bg-${type} border-0" role="alert" aria-live="assertive" aria-atomic="true" data-bs-delay="${delay}">
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;

    // Append the toast HTML to the container
    toastContainer.insertAdjacentHTML('beforeend', toastHtml);

    // Get the newly added toast element
    const toastElement = document.getElementById(toastId);
    if (toastElement) {
        const toast = new bootstrap.Toast(toastElement);

        // Remove the toast element from the DOM after it's hidden
        toastElement.addEventListener('hidden.bs.toast', function () {
            toastElement.remove();
        });

        toast.show();
    }
}

// Example Usage (can be removed later):
// showToast('This is a success message!', 'success');
// showToast('This is an error message.', 'danger');
// showToast('This is just info.', 'info');
