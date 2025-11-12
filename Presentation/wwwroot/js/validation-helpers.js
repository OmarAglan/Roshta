// wwwroot/js/validation-helpers.js

// --- Reusable Validation Helper Functions ---

/**
 * Validates a date input to ensure it is in the future.
 * @param {HTMLInputElement} inputElement The date input element.
 * @param {HTMLElement} validationSpan The span element to display validation messages.
 * @param {string} [errorMessagePrefix='Date'] Prefix for the error message (e.g., 'Expiry Date').
 */
function validateDateIsFuture(inputElement, validationSpan, errorMessagePrefix = 'Date') {
    if (!inputElement || !validationSpan) return; 

    const dateValue = inputElement.value;
    validationSpan.textContent = '';
    inputElement.classList.remove('is-invalid');

    if (dateValue) {
        try {
            const today = new Date();
            today.setUTCHours(0, 0, 0, 0);
            const parts = dateValue.split('-');
            const inputDate = new Date(Date.UTC(parts[0], parts[1] - 1, parts[2]));

            if (inputDate <= today) {
                validationSpan.textContent = `${errorMessagePrefix} must be in the future.`;
                inputElement.classList.add('is-invalid');
            }
        } catch (e) {
            console.error("Error parsing date:", e);
            validationSpan.textContent = 'Invalid date format.';
            inputElement.classList.add('is-invalid');
        }
    }
}

/**
 * Validates a date input to ensure it is in the past.
 * @param {HTMLInputElement} inputElement The date input element.
 * @param {HTMLElement} validationSpan The span element to display validation messages.
 * @param {string} [errorMessagePrefix='Date'] Prefix for the error message (e.g., 'Date of Birth').
 */
function validateDateIsPast(inputElement, validationSpan, errorMessagePrefix = 'Date') {
    if (!inputElement || !validationSpan) return;
    const dateValue = inputElement.value;
    validationSpan.textContent = ''; 
    inputElement.classList.remove('is-invalid');

    if (dateValue) {
        try {
            const today = new Date();
            today.setUTCHours(0, 0, 0, 0);
            const parts = dateValue.split('-');
            const inputDate = new Date(Date.UTC(parts[0], parts[1] - 1, parts[2]));

            if (inputDate >= today) {
                validationSpan.textContent = `${errorMessagePrefix} must be in the past.`;
                inputElement.classList.add('is-invalid');
            }
        } catch (e) {
            validationSpan.textContent = 'Invalid date format.';
            inputElement.classList.add('is-invalid');
        }
    }
}

/**
 * Validates that at least one of two contact fields (phone, email) is filled.
 * @param {HTMLInputElement} phoneInputElement The phone input element.
 * @param {HTMLInputElement} emailInputElement The email input element.
 * @param {HTMLElement} phoneValidationSpan The phone validation message span.
 * @param {HTMLElement} emailValidationSpan The email validation message span.
 */
function validateContactMethodRequired(phoneInputElement, emailInputElement, phoneValidationSpan, emailValidationSpan) {
    const validationMessage = "Please provide at least one contact method (Phone or Email).";
    const phoneValue = phoneInputElement ? phoneInputElement.value.trim() : '';
    const emailValue = emailInputElement ? emailInputElement.value.trim() : '';

    if (!phoneValue && !emailValue) {
        if (phoneValidationSpan) phoneValidationSpan.textContent = validationMessage;
        if (emailValidationSpan) emailValidationSpan.textContent = validationMessage;
        if (phoneInputElement) phoneInputElement.classList.add('is-invalid');
        if (emailInputElement) emailInputElement.classList.add('is-invalid');
    } else {
        if (phoneValidationSpan) phoneValidationSpan.textContent = '';
        if (emailValidationSpan) emailValidationSpan.textContent = '';
        if (phoneInputElement) phoneInputElement.classList.remove('is-invalid');
        if (emailInputElement) emailInputElement.classList.remove('is-invalid');
    }
}

/**
 * Validates the format of an Egyptian phone number (e.g., +201XXXXXXXXX).
 * @param {HTMLInputElement} inputElement The phone input element.
 * @param {HTMLElement} validationSpan The span element to display validation messages.
 */
function validatePhoneNumberFormat(inputElement, validationSpan) {
    if (!inputElement || !validationSpan) return;
    const phoneValue = inputElement.value.trim();
    // Regex for Egyptian format like +201012345678 or +201112345678 etc. (+201 followed by 9 digits)
    const phoneRegex = /^\+201[0-9]{9}$/;

    // Clear previous format error, but respect errors from other validators (like required contact)
    // Only add/remove format error message and is-invalid if the field has content.
    if (phoneValue) { 
        if (!phoneRegex.test(phoneValue)) {
            // Updated error message for specific format
            validationSpan.textContent = 'Invalid format (e.g., +201XXXXXXXXX).';
            inputElement.classList.add('is-invalid');
        } else {
            // If format is valid, clear ONLY the format error message 
            // ONLY IF the current message IS the format error message.
            // Otherwise, another validator (like required contact) might be active.
            if (validationSpan.textContent.startsWith('Invalid format')) {
                 validationSpan.textContent = '';
            }
            // We only remove is-invalid if no other validation message exists for this span
            if (!validationSpan.textContent) { 
                 inputElement.classList.remove('is-invalid');
            }
        }
    } else {
         // If the field is empty, clear a format-specific error, but leave others.
         if (validationSpan.textContent.startsWith('Invalid format')) {
            validationSpan.textContent = '';
         }
         // Only remove is-invalid if no other validation message exists
         if (!validationSpan.textContent) { 
            inputElement.classList.remove('is-invalid');
         }         
    }
}

/**
 * Validates that an input field is not empty.
 * @param {HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement} inputElement The input element.
 * @param {HTMLElement} validationSpan The span element to display validation messages.
 * @param {string} [fieldName='Field'] Name of the field for the error message.
 * @returns {boolean} True if the field is valid (not empty), false otherwise.
 */
function validateRequiredField(inputElement, validationSpan, fieldName = 'Field') {
    if (!inputElement) return false; // Cannot validate without an input element

    const value = inputElement.value.trim();
    const requiredMessage = `${fieldName} is required.`;
    let isValid = true;

    if (!value) {
        // If a span exists, update it
        if (validationSpan) {
            validationSpan.textContent = requiredMessage;
        }
        inputElement.classList.add('is-invalid');
        isValid = false;
    } else {
        // If a span exists and shows the required message, clear it
        if (validationSpan && validationSpan.textContent === requiredMessage) {
            validationSpan.textContent = '';
        }
        // Only remove 'is-invalid' if no other validation message exists (check span if available)
        if (!validationSpan || !validationSpan.textContent) {
            inputElement.classList.remove('is-invalid');
        }
        isValid = true;
    }
    return isValid; // Return the validity status
}

/**
 * Validates that date B is after date A, if both are provided.
 * @param {HTMLInputElement} dateAInputElement The first date input (e.g., Next Appointment).
 * @param {HTMLInputElement} dateBInputElement The second date input (e.g., Expiry Date).
 * @param {HTMLElement} dateAValidationSpan Validation span for date A.
 * @param {HTMLElement} dateBValidationSpan Validation span for date B.
 * @param {string} dateAName Name for date A in error messages.
 * @param {string} dateBName Name for date B in error messages.
 */
function validateDateBAfterDateA(dateAInputElement, dateBInputElement, dateAValidationSpan, dateBValidationSpan, dateAName, dateBName) {
    if (!dateAInputElement || !dateBInputElement || !dateAValidationSpan || !dateBValidationSpan) return;

    const dateAValue = dateAInputElement.value;
    const dateBValue = dateBInputElement.value;
    const errorMessage = `${dateBName} must be after ${dateAName}.`;

    // Clear previous cross-field error messages first
    if (dateAValidationSpan.textContent === errorMessage) dateAValidationSpan.textContent = '';
    if (dateBValidationSpan.textContent === errorMessage) dateBValidationSpan.textContent = '';
    // Only remove is-invalid if no other validation error exists for the spans
    if (!dateAValidationSpan.textContent) dateAInputElement.classList.remove('is-invalid');
    if (!dateBValidationSpan.textContent) dateBInputElement.classList.remove('is-invalid');

    // Only perform check if both dates have values
    if (dateAValue && dateBValue) {
        try {
            const partsA = dateAValue.split('-');
            const dateA = new Date(Date.UTC(partsA[0], partsA[1] - 1, partsA[2]));

            const partsB = dateBValue.split('-');
            const dateB = new Date(Date.UTC(partsB[0], partsB[1] - 1, partsB[2]));

            if (dateB <= dateA) {
                // Show error on both fields involved in the cross-validation
                dateAValidationSpan.textContent = errorMessage;
                dateBValidationSpan.textContent = errorMessage;
                dateAInputElement.classList.add('is-invalid');
                dateBInputElement.classList.add('is-invalid');
            }
        } catch (e) {
            console.error("Error parsing dates for cross-validation:", e);
            // Don't add a new error here, let the individual field format validation handle it
        }
    }
}

// --- Utility Functions ---

/**
 * Debounce function: Limits the rate at which a function can fire.
 * @param {Function} func The function to debounce.
 * @param {number} delay Delay in milliseconds.
 * @returns {Function} The debounced function.
 */
function debounce(func, delay) {
    let debounceTimer;
    return function() {
        const context = this;
        const args = arguments;
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(() => func.apply(context, args), delay);
    }
}

/**
 * Performs a basic validation check to see if the input looks like a phone number or email.
 * Does not validate if the input is empty (let required validator handle that).
 * @param {HTMLInputElement} inputElement The input element.
 * @param {HTMLElement} validationSpan The span element to display validation messages.
 */
function validateBasicContactFormat(inputElement, validationSpan) {
    if (!inputElement || !validationSpan) return;
    const value = inputElement.value.trim();
    const formatErrorMessage = 'Please enter a valid phone number or email format.';

    // Only validate format if the field is not empty
    if (value) {
        // Basic checks: contains '@' and '.' for email, or looks like a phone number (digits, +, -, spaces)
        const looksLikeEmail = value.includes('@') && value.includes('.');
        const looksLikePhone = /^[+\d][\d\s-]*\d$/.test(value); // Starts with + or digit, ends with digit, allows digits, space, hyphen in between

        if (!looksLikeEmail && !looksLikePhone) {
            validationSpan.textContent = formatErrorMessage;
            inputElement.classList.add('is-invalid');
        } else {
            // Clear only the format error message if it's currently displayed
            if (validationSpan.textContent === formatErrorMessage) {
                validationSpan.textContent = '';
            }
            // Only remove 'is-invalid' if no other validation message exists for this span
            if (!validationSpan.textContent) {
                inputElement.classList.remove('is-invalid');
            }
        }
    } else {
        // If the field is empty, clear the format error message
        if (validationSpan.textContent === formatErrorMessage) {
            validationSpan.textContent = '';
        }
        // Only remove 'is-invalid' if no other validation message exists
        if (!validationSpan.textContent) {
            inputElement.classList.remove('is-invalid');
        }
    }
}

/**
 * Validates if a string contains only digits and its length is within a specified range.
 * Does not validate if the input is empty (let required validator handle that).
 * @param {HTMLInputElement} inputElement The input element.
 * @param {HTMLElement} validationSpan The span element to display validation messages.
 * @param {number} minLength The minimum required length (inclusive).
 * @param {number} maxLength The maximum allowed length (inclusive).
 * @param {string} [fieldName='Field'] Name of the field for the error message.
 */
function validateNumericRange(inputElement, validationSpan, minLength, maxLength, fieldName = 'Field') {
    if (!inputElement || !validationSpan) return false; // Return validity
    const value = inputElement.value.trim();
    const digitRegex = /^\d+$/;
    const formatErrorMessage = `${fieldName} must contain only digits.`;
    const lengthErrorMessage = `${fieldName} must be between ${minLength} and ${maxLength} digits long.`;
    let isValid = true;

    // Only validate format if the field is not empty
    if (value) {
        let currentError = '';
        if (!digitRegex.test(value)) {
            currentError = formatErrorMessage;
            isValid = false;
        } else if (value.length < minLength || value.length > maxLength) {
            currentError = lengthErrorMessage;
            isValid = false;
        }

        // Update validation message and class
        if (!isValid) {
            validationSpan.textContent = currentError;
            inputElement.classList.add('is-invalid');
        } else {
            // Clear only our specific error messages
            if (validationSpan.textContent === formatErrorMessage || validationSpan.textContent === lengthErrorMessage) {
                validationSpan.textContent = '';
            }
             // Only remove 'is-invalid' if no other validation message exists
            if (!validationSpan.textContent) {
                inputElement.classList.remove('is-invalid');
            }
        }
    } else {
        // If the field is empty, clear our specific error messages
        if (validationSpan.textContent === formatErrorMessage || validationSpan.textContent === lengthErrorMessage) {
            validationSpan.textContent = '';
        }
        // If empty and no other message, remove invalid class (required validator might handle this)
        if (!validationSpan.textContent) {
            inputElement.classList.remove('is-invalid');
        }
        // It's considered valid from *this* validator's perspective if empty
        isValid = true; 
    }
    return isValid; // Return the validity status
}

/**
 * Validates if a value is a positive number (integer or decimal).
 * Does not validate if the input is empty.
 * @param {HTMLInputElement} inputElement The input element.
 * @param {HTMLElement} validationSpan The span element to display validation messages.
 * @param {string} [fieldName='Field'] Name of the field for the error message.
 */
function validatePositiveNumber(inputElement, validationSpan, fieldName = 'Field') {
    if (!inputElement || !validationSpan) return false;
    const value = inputElement.value.trim();
    const errorMessage = `${fieldName} must be a positive number.`;
    let isValid = true;

    if (value) {
        const numberValue = parseFloat(value);
        if (isNaN(numberValue) || numberValue <= 0) {
            validationSpan.textContent = errorMessage;
            inputElement.classList.add('is-invalid');
            isValid = false;
        } else {
             // Clear only our specific error message
            if (validationSpan.textContent === errorMessage) {
                validationSpan.textContent = '';
            }
             // Only remove 'is-invalid' if no other validation message exists
            if (!validationSpan.textContent) {
                inputElement.classList.remove('is-invalid');
            }
        }
    } else {
         // Clear our specific error message if the field is empty
         if (validationSpan.textContent === errorMessage) {
            validationSpan.textContent = '';
         }
        // If empty and no other message, remove invalid class
        if (!validationSpan.textContent) {
            inputElement.classList.remove('is-invalid');
         }
         isValid = true; // Valid from this validator's perspective if empty
    }
     return isValid;
}

/**
 * Validates if a value is a non-negative integer (0, 1, 2...).
 * Does not validate if the input is empty.
 * @param {HTMLInputElement} inputElement The input element.
 * @param {HTMLElement} validationSpan The span element to display validation messages.
 * @param {string} [fieldName='Field'] Name of the field for the error message.
 */
function validateNonNegativeInteger(inputElement, validationSpan, fieldName = 'Field') {
    if (!inputElement || !validationSpan) return false;
    const value = inputElement.value.trim();
    const errorMessage = `${fieldName} must be a non-negative integer (0, 1, 2...).`;
    const integerRegex = /^\d+$/;
    let isValid = true;

    if (value) {
        if (!integerRegex.test(value)) { // Checks for digits only, implicitly >= 0
            validationSpan.textContent = errorMessage;
            inputElement.classList.add('is-invalid');
            isValid = false;
        } else {
             // Clear only our specific error message
            if (validationSpan.textContent === errorMessage) {
                validationSpan.textContent = '';
            }
             // Only remove 'is-invalid' if no other validation message exists
            if (!validationSpan.textContent) {
                inputElement.classList.remove('is-invalid');
            }
        }
    } else {
        // Clear our specific error message if the field is empty
         if (validationSpan.textContent === errorMessage) {
            validationSpan.textContent = '';
         }
        // If empty and no other message, remove invalid class
        if (!validationSpan.textContent) {
            inputElement.classList.remove('is-invalid');
         }
        isValid = true; // Valid from this validator's perspective if empty
    }
    return isValid;
}

/**
 * Checks if a form is client-side valid by looking for visible validation messages
 * and inputs with the 'is-invalid' class within a specific container (usually the form itself).
 * @param {HTMLElement} formElement The form element or a container element.
 * @returns {boolean} True if no validation errors are found, false otherwise.
 */
function isFormClientSideValid(formElement) {
    if (!formElement) return true; // Default to valid if no form provided

    // Check for any validation spans with text content
    // Assumes validation spans have class 'text-danger' and are direct children or within standard structures
    const validationSpans = formElement.querySelectorAll('span.text-danger[data-valmsg-for], span.text-danger[id$="Validation"]');
    for (let span of validationSpans) {
        if (span.textContent.trim() !== '') {
            // console.log('Form invalid due to span:', span);
            return false; // Found an error message
        }
    }

    // Check for any input elements with the 'is-invalid' class
    const invalidInputs = formElement.querySelectorAll('.is-invalid');
    if (invalidInputs.length > 0) {
        // console.log('Form invalid due to input:', invalidInputs[0]);
        return false; // Found an input marked as invalid
    }

    return true; // No errors found
}

/**
 * Sets up real-time uniqueness check for the medication name input field via AJAX.
 * Assumes input has id="Medication_Name" and a validation message span exists
 * (e.g., <span data-valmsg-for="Medication.Name"></span>).
 * @param {string} checkUrl The base URL to the Page Handler (e.g., '/Medications/Create')
 *                          The handler name 'CheckMedicationNameUnique' will be appended.
 * @param {number|null} [currentId=null] The ID of the item being edited, to exclude it from the check. Pass null for create.
 */
function setupMedicationNameUniquenessCheck(checkUrl, currentId = null) {
    const nameInput = document.getElementById('Medication_Name');
    // Find the validation message placeholder associated with the Name input
    const validationSpan = document.querySelector('span[data-valmsg-for="Medication.Name"]');

    if (!nameInput || !validationSpan) {
        console.warn('Medication name input or validation span not found for uniqueness check.');
        return;
    }

    const uniquenessErrorMessage = 'Medication name already exists.';
    let debounceTimer; // For debouncing input checks (optional, using blur for now)

    nameInput.addEventListener('blur', async function() { // Using 'blur' for simplicity, could use debounced 'input'
        const name = nameInput.value.trim();

        // Clear previous uniqueness error first
        if (validationSpan.textContent === uniquenessErrorMessage) {
            validationSpan.textContent = '';
             // Only remove is-invalid if no other validation message exists
            if (!validationSpan.textContent) {
                 nameInput.classList.remove('is-invalid');
            }
        }

        if (!name) {
            // Don't check empty strings, let 'Required' validator handle it
            return;
        }

        try {
            // Include currentId in the query if provided (for edit checks)
            const idQueryParam = currentId !== null ? `&currentId=${currentId}` : '';
            const handlerUrl = `${checkUrl}?handler=CheckMedicationNameUnique&name=${encodeURIComponent(name)}${idQueryParam}`;
            const response = await fetch(handlerUrl);

            if (!response.ok) {
                throw new Error(`Network response was not ok: ${response.statusText}`);
            }

            const result = await response.json();

            if (!result.isUnique) {
                validationSpan.textContent = uniquenessErrorMessage;
                nameInput.classList.add('is-invalid');
            }
            // If it is unique, we've already cleared the message above.

        } catch (error) {
            console.error('Error checking medication name uniqueness:', error);
            // Optionally display a generic error message to the user
            // validationSpan.textContent = 'Error checking uniqueness.';
            // nameInput.classList.add('is-invalid');
        }
    });
}

/**
 * Sets up real-time uniqueness check for the patient contact info input field via AJAX.
 * Assumes input has id="Patient_ContactInfo" and a validation message span exists
 * (e.g., <span data-valmsg-for="Patient.ContactInfo"></span>).
 * @param {string} checkUrl The base URL to the Page Handler (e.g., '/Patients/Create')
 *                          The handler name 'CheckContactInfoUnique' will be appended.
 * @param {number|null} [currentId=null] The ID of the item being edited, to exclude it from the check. Pass null for create.
 */
function setupPatientContactInfoUniquenessCheck(checkUrl, currentId = null) {
    const contactInput = document.getElementById('Patient_ContactInfo');
    const validationSpan = document.querySelector('span[data-valmsg-for="Patient.ContactInfo"]');

    if (!contactInput || !validationSpan) {
        console.warn('Patient contact info input or validation span not found for uniqueness check.');
        return;
    }

    const uniquenessErrorMessage = 'Contact Info already exists.';

    contactInput.addEventListener('blur', async function() { 
        const contactInfo = contactInput.value.trim();

        // Clear previous uniqueness error first
        if (validationSpan.textContent === uniquenessErrorMessage) {
            validationSpan.textContent = '';
            if (!validationSpan.textContent) { // Remove class only if no other errors
                contactInput.classList.remove('is-invalid');
            }
        }

        if (!contactInfo) {
            return; // Don't check empty strings
        }

        try {
            const idQueryParam = currentId !== null ? `&currentId=${currentId}` : '';
            // Use the correct handler name
            const handlerUrl = `${checkUrl}?handler=CheckContactInfoUnique&contactInfo=${encodeURIComponent(contactInfo)}${idQueryParam}`;
            const response = await fetch(handlerUrl);

            if (!response.ok) {
                throw new Error(`Network response was not ok: ${response.statusText}`);
            }

            const result = await response.json();

            if (!result.isUnique) {
                validationSpan.textContent = uniquenessErrorMessage;
                contactInput.classList.add('is-invalid');
            }

        } catch (error) {
            console.error('Error checking patient contact info uniqueness:', error);
        }
    });
}

/**
 * Sets up real-time uniqueness check for the patient name input field via AJAX.
 * Assumes input has id="Patient_Name" and a validation message span exists
 * (e.g., <span data-valmsg-for="Patient.Name"></span>).
 * @param {string} checkUrl The base URL to the Page Handler (e.g., '/Patients/Create')
 *                          The handler name 'CheckNameUnique' will be appended.
 * @param {number|null} [currentId=null] The ID of the item being edited, to exclude it from the check. Pass null for create.
 */
function setupPatientNameUniquenessCheck(checkUrl, currentId = null) {
    const nameInput = document.getElementById('Patient_Name');
    const validationSpan = document.querySelector('span[data-valmsg-for="Patient.Name"]');

    if (!nameInput || !validationSpan) {
        console.warn('Patient name input or validation span not found for uniqueness check.');
        return;
    }

    const uniquenessErrorMessage = 'Patient Name already exists.';

    nameInput.addEventListener('blur', async function() { 
        const name = nameInput.value.trim();

        // Clear previous uniqueness error first
        if (validationSpan.textContent === uniquenessErrorMessage) {
            validationSpan.textContent = '';
            if (!validationSpan.textContent) { // Remove class only if no other errors
                nameInput.classList.remove('is-invalid');
            }
        }

        if (!name) {
            return; // Don't check empty strings
        }

        try {
            const idQueryParam = currentId !== null ? `&currentId=${currentId}` : '';
            // Use the correct handler name
            const handlerUrl = `${checkUrl}?handler=CheckNameUnique&name=${encodeURIComponent(name)}${idQueryParam}`;
            const response = await fetch(handlerUrl);

            if (!response.ok) {
                throw new Error(`Network response was not ok: ${response.statusText}`);
            }

            const result = await response.json();

            if (!result.isUnique) {
                validationSpan.textContent = uniquenessErrorMessage;
                nameInput.classList.add('is-invalid');
            }

        } catch (error) {
            console.error('Error checking patient name uniqueness:', error);
        }
    });
}
