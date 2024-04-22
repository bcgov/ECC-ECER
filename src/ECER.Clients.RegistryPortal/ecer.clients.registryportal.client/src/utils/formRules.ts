// contains helper functions for form validation in vue.
//
// Ex. use case to create an email input that is required
//  <v-text-field
//     v-model="contactEdit.alternatePhoneExtension"
//     :rules="[rules.email(), rules.required('custom message here')]"
//  </v-text-field>
//
// Example SchoolContact.vue
//
// REMEMBER to do the following in your .vue file
//  import * as Rule from @/utils/institute/form
//  under data do rules: Rules <- allows you to use in <template>.

import { DateTime } from "luxon";

/**
 * Rule for emails
 * @param {String} message
 * @returns Function
 */
const email = (message = "Enter a valid email address in the format 'name@email.com'") => {
  return (v: string) => !v || /.+@.+\..+/.test(v) || message;
};

/**
 * Rule to check input is a number
 * @param {String} message
 * @returns Function
 */
const number = (message = "Must be a number") => {
  return (v: string) => !v || /^\d+$/.test(v) || message;
};

/**
 * Form input must not contain special characters
 *
 * @param {String} [message]
 * @returns {(value: string) => true|string}
 */
const noSpecialCharactersAddress =
  (
    message = "Special characters currently aren’t accepted, but we recognize their importance and are working on an update. For now, please remove or replace them.",
  ) =>
  (v: string) =>
    !v || !/[^A-Za-z0-9\s-.#/]/.test(v) || message;

const noSpecialCharactersContactTitle =
  (message = "Remove or replace any special characters in this field.") =>
  (v: string) =>
    !v || !/[^A-Za-z.'\s-&()]/.test(v) || message;

const noSpecialCharactersContactName =
  (message = "Remove or replace any special characters in this field.") =>
  (v: string) =>
    !v || !/[^A-Za-z.'\s-]/.test(v) || message;

/**
 * Rule for phone numbers also works for fax numbers too
 * @param {String} message
 * @returns Function
 */
const phoneNumber = (message = "Enter a valid, 10-digit phone number") => {
  return (v: string) => !v || /^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$/.test(v) || message;
};

/**
 * Rule for postalCodes
 * @param {String} message
 * @returns Function
 */
const postalCode = (message = "Enter your postal code in the format 'A1A 1A1'") => {
  return (v: string) => /^[ABCEGHJ-NPRSTVXY]\d[ABCEGHJ-NPRSTV-Z][ -]?\d[ABCEGHJ-NPRSTV-Z]\d$/i.test(v) || message;
};

/**
 * Rule for checkbox
 * @param {String} message
 * @returns Function
 */
const hasCheckbox = (message = "You must check the box") => {
  return (v: boolean) => !!v || message;
};

/**
 * Rule required v.trim prevents ' ' from being valid
 * @param {String} message
 * @returns Function
 */
const required = (message = "This field is required") => {
  return (v: string | number) => {
    if (typeof v === "number") {
      return !!v || message;
    } else {
      return !!(v && v?.trim()) || message;
    }
  };
};

/**
 * Rule required for radio buttons
 * @param {String} message
 * @returns Function
 */
const requiredRadio = (message = "This field is required") => {
  return (v: boolean | string) => v !== undefined || message;
};

/**
 * Custom endDate Rule! Checks that we have start date and that end date
 * happens after start date. Date format should be 2022-12-10 YYYY-MM-DD.
 * @param {String} effectiveDate
 * @param {String} expiryDate
 * @returns {String|Boolean}
 */
const endDateRule = (effectiveDate: string, expiryDate: string, message = "End date cannot be before start date") => {
  if (effectiveDate && expiryDate) {
    const effDate = DateTime.fromISO(effectiveDate);
    const expDate = DateTime.fromISO(expiryDate);

    return expDate < effDate || expDate === effDate || message;
  }

  return true;
};

/**
 * Rule for website url
 * @param {String} message
 * @returns Function
 */
const website = (message = "Website must be valid and secure (i.e., https)") => {
  return (v: string) => !v || /^https:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_+.~#?&/=]*)$/.test(v) || message;
};

export {
  email,
  endDateRule,
  hasCheckbox,
  noSpecialCharactersAddress,
  noSpecialCharactersContactName,
  noSpecialCharactersContactTitle,
  number,
  phoneNumber,
  postalCode,
  required,
  requiredRadio,
  website,
};
