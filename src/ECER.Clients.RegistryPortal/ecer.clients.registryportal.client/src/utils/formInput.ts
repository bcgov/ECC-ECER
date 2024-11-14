// helper function to use on input fields
// how to use: example SchoolContact.vue
// import {isNumber} from '@/utils/institute/formInput';
// under methods section in vue: isNumber (allows us to use in <template>)
// <v-text-field @keypress="isNumber($event)" />

/**
 * Will only allow numbers in an input field.
 * @param {Object} event
 * @returns void
 */
const isNumber = function (event: KeyboardEvent | InputEvent): void {
  const charCode = event instanceof KeyboardEvent ? event.key : "";

  // Check if the key pressed is not a number and not a valid input event
  if (
    (event instanceof KeyboardEvent && (charCode > "9" || charCode < "0") && charCode !== "Backspace") ||
    (event instanceof InputEvent && isNaN(Number((event.target as HTMLInputElement).value)))
  ) {
    event.preventDefault();
  }
};

/**
 * Will not allow special characters for names in an input field. Acceptable characters include ' and -
 * @param {Object} event
 * @returns void
 */
const validContactNameCharacter = function (event: KeyboardEvent): void {
  const charCode = event instanceof KeyboardEvent ? event.key : "";

  // Check if the key pressed is not a number and not a valid input event
  if (event instanceof KeyboardEvent && /[^a-zA-Z\u00C0-\u017F\s'-]/.test(charCode)) {
    event.preventDefault();
  }
};

export { validContactNameCharacter, isNumber };
