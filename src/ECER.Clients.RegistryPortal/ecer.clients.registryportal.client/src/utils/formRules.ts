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
    !v || !/[^a-zA-Z\u00C0-\u017F\s'-]/.test(v) || message;

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
  return (v: boolean | string | null | undefined) => (v !== undefined && v !== null) || message;
};

/**
 * Rule to check that at least one option was selected in a dropdown/multi select checkboxes
 * @param {String} message
 * @returns Function
 */
const atLeastOneOptionRequired = (message = "Please select at least one option") => {
  return (v: any[]) => v.length > 0 || message;
};
/**
 * Checks whether input is x years from a specific date
 * Date format should be 2022-12-10 YYYY-MM-DD.
 * @param {String} targetDate
 * @param {Number} years
 * @returns {String|Boolean}
 */
const dateRuleRange = (targetDate: string, years: number, message = `Date should be within ${years} years`) => {
  return (v: string) => {
    if (v && targetDate) {
      const input = DateTime.fromJSDate(new Date(v));
      const target = DateTime.fromISO(targetDate);
      const differenceInYears = Math.abs(input.diff(target, "years").years);

      return differenceInYears < years || message;
    }

    return true;
  };
};

/**
 * Validates that a date string falls within a specified range.
 *
 * @param {string} startDate - The start date of the valid range (ISO 8601 format).
 * @param {string} endDate - The end date of the valid range (ISO 8601 format).
 * @param {string} [message="Date should be between ${startDate} and ${endDate}"] - The error message to return if the date is outside the valid range.
 * @returns {boolean|string} - Returns `true` if the date is within the valid range, otherwise returns the error message.
 */
const dateBetweenRule = (startDate: string, endDate: string, message = `Date should be between ${startDate} and ${endDate}`) => {
  return (v: string) => {
    if (v && startDate && endDate) {
      const end = DateTime.fromISO(endDate);
      const start = DateTime.fromISO(startDate);
      const input = DateTime.fromJSDate(new Date(v));

      return (input <= end && input >= start) || message;
    }

    return true;
  };
};
/**
 * Validates that a date string is not before a specified target date.
 *
 * @param {string} targetDate - The target date (ISO 8601 format).
 * @param {string} [message="End date cannot be before start date"] - The error message to return if the date is before the target date.
 * @returns {boolean|string} - Returns `true` if the date is not before the target date, otherwise returns the error message.
 */
const dateBeforeRule = (targetDate: string, message = "End date cannot be before start date") => {
  return (v: string) => {
    if (v && targetDate) {
      const input = DateTime.fromJSDate(new Date(v));
      const target = DateTime.fromISO(targetDate);

      return input >= target || message;
    }

    return true;
  };

  return true;
};

/**
 * Validates that a date string is not greater than today's date.
 *
 * @param {string} [message="Date cannot be in the future"] - The error message to return if the date is greater than today.
 * @returns {boolean|string} - Returns `true` if the date is not greater than today, otherwise returns the error message.
 */
const futureDateNotAllowedRule = (message = "Date must be before today") => {
  return (v: string) => {
    if (v) {
      const input = DateTime.fromJSDate(new Date(v));
      const today = DateTime.now().startOf("day");
      return input <= today || message;
    }

    return true;
  };
};

/**
 * conditional wrapper for form rules. If condition is met then return rule function
 * otherwise ignore by returning true
 * @param {boolean} condition
 * @param {Function} rule
 * @returns {Boolean|ValidationRule$1}
 */
const conditionalWrapper = (condition: boolean, rule: any) => {
  return condition ? rule : true;
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
  atLeastOneOptionRequired,
  conditionalWrapper,
  dateBeforeRule,
  dateBetweenRule,
  dateRuleRange,
  email,
  futureDateNotAllowedRule,
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
