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
  return (v: string) => {
    if (!v) return true;
    // RFC 5322 compliant pattern for most email addresses
    const pattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return pattern.test(v) || message;
  };
};
/**
 * Rule to check input is a number
 * @param {String} message
 * @returns Function
 */
const number = (message = "Must be a number") => {
  return (v: string) => !v || /^\d+$/.test(v) || message;
};

const validContactName =
  (message = "Remove or replace any special characters in this field.") =>
  (v: string) =>
    !v || !/[^a-zA-Z\u00C0-\u017F\s'-]/.test(v) || message;

/**
 * Rule for phone numbers
 *
 * The validator checks that the input is empty or:
 * - 7 to 20 characters long,
 * - Optionally starts with a '+',
 * - Contains only digits, spaces, and dashes.
 *
 * @param {string} message
 * @returns Function
 */

const phoneNumber = (message = "Enter a valid phone number") => {
  return (v: string) => !v || /^(?=.{7,20}$)(?:\+)?[\d\s-]+$/.test(v) || message;
};

/**
 * Rule for postalCodes
 * @param {String} message
 * @returns Function
 */
const postalCode = (message = "Enter your postal code in the format 'A1A 1A1'") => {
  return (v: string) => /^[ABCEGHJ-NPRSTVXY]\d[ABCEGHJ-NPRSTV-Z][ -]?\d[ABCEGHJ-NPRSTV-Z]\d$/i.test(v) || message;
};

const mustExistInList = <T>(list: T[], key: keyof T, message: string = "Please select a valid option") => {
  return (v: string) => {
    return list.some((item) => item[key] === v) || message;
  };
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
const required = (message = "This field is required", property?: string) => {
  return (v: string | number | Record<string, any> | null) => {
    if (typeof v === "number") {
      return !!v || message;
    } else if (typeof v === "string") {
      return !!v.trim() || message;
    } else if (typeof v === "object" && v !== null && property) {
      return !!v[property]?.toString().trim() || message;
      // this handles case where we have a date object with undefined Property.
    } else if (typeof v === "object" && v !== null && !property) {
      return !!v?.toString() || message;
    }
    return message;
  };
};

/**
 * Validates if a number has a specified number of decimal places.
 *
 * @param {number} [decimal=2] - The maximum number of decimal places allowed.
 * @param {string} [message=`Input a number up to ${decimal} decimal places. Ex. ${Number(1).toFixed(decimal)}`] - The error message to display if the validation fails.
 * @returns {function(string): boolean|string} A validation function that accepts a string and returns true if valid, or the error message if invalid.
 */
const numberToDecimalPlace = (decimal = 2, message = `Input a number up to ${decimal} decimal places. Ex. ${Number(1).toFixed(decimal)}`) => {
  const regex = new RegExp(`^\\d+(\\.\\d{0,${decimal}})?$`);
  return (v: string) => !v || regex.test(v) || message;
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
 * Rule to ensure two values are not the same
 * @param {String} otherValue - The value to compare against.
 * @param {String} message - The error message.
 * @returns {Function}
 */
const notSameAs = (otherValue: string, message = "Both values must be different") => {
  return (v: string) => v !== otherValue || message;
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
  validContactName,
  mustExistInList,
  number,
  numberToDecimalPlace as numberToDecimalPlaces,
  phoneNumber,
  postalCode,
  required,
  requiredRadio,
  website,
  notSameAs,
};
