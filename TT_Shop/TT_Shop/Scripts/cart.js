function saveCheckboxState() {
    let selectedProductIds = [];
    document.querySelectorAll('input[name="selectedProducts"]:checked').forEach(function (checkbox) {
        selectedProductIds.push(parseInt(checkbox.value));
    });

    fetch('/Cart/SaveSelectedProducts', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify({ selectedProductIds: selectedProductIds })
    })
        .then(response => response.json())
        .then(data => {
            if (!data.success) {
                console.error('Failed to save selected products');
            }
        })
        .catch(error => console.error('Error:', error));
}

function calculateTotal() {
    var checkboxes = document.querySelectorAll('input[name="selectedProducts"]:checked');
    var total = 0;

    if (checkboxes.length > 0) {
        checkboxes.forEach(function (checkbox) {
            var price = parseFloat(checkbox.getAttribute('data-price')) || 0;
            var quantity = parseInt(checkbox.getAttribute('data-quantity')) || 0;
            total += price * quantity;
        });
    }

    document.getElementById('totalPrice').innerText = (total > 0 ? total.toLocaleString('vi-VN') : "0") + " VND";
}

function updateQuantity(productId, quantity) {
    fetch('/Cart/UpdateQuantity', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify({ cartItemId: productId, quantity: quantity })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Update the data-quantity attribute of the checkbox
                var checkbox = document.querySelector('input[name="selectedProducts"][value="' + productId + '"]');
                if (checkbox) {
                    checkbox.setAttribute('data-quantity', quantity);
                }

                // Update the total price for the specific item
                var price = parseFloat(checkbox.getAttribute('data-price')) || 0;
                var totalCell = checkbox.closest('tr').querySelector('.item-total');
                if (totalCell) {
                    totalCell.innerText = (price * quantity).toLocaleString('vi-VN') + " VND";
                }

                // Recalculate the overall total
                calculateTotal();
            } else {
                alert(data.message);
            }
        })
        .catch(error => console.error('Error:', error));
}


function loadSelectedProducts() {
    fetch('/Cart/GetSelectedProducts', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                var selectedProductIds = data.selectedProductIds;
                selectedProductIds.forEach(function (productId) {
                    var checkbox = document.querySelector('input[name="selectedProducts"][value="' + productId + '"]');
                    if (checkbox) {
                        checkbox.checked = true;
                    }
                });
                calculateTotal(); // Calculate total after loading the state
            }
        })
        .catch(error => console.error('Error:', error));
}

document.addEventListener("DOMContentLoaded", function () {
    loadSelectedProducts(); // Load the state when the page is loaded
});

// Save the checkbox state whenever the user changes the selection
document.addEventListener('change', function (e) {
    if (e.target && e.target.name === 'selectedProducts') {
        saveCheckboxState();
        calculateTotal();
    }
});
