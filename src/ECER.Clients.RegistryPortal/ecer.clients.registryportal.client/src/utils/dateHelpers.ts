import { DateTime } from "luxon";

/**
 * Helper functions to generate dynamic dates based on today's date
 * All dates are returned in ISO format with time set to 00:00:00
 */

/**
 * Get today's date in ISO format
 */
export const getTodayDate = (): string => {
  return DateTime.now().toISO();
};

/**
 * Get a date that is 2 years from today
 */
export const getTwoYearsFromToday = (): string => {
  return DateTime.now().plus({ years: 5 }).toISO();
};

/**
 * Get a date that is 2 years ago
 */
export const getTwoYearsAgo = (): string => {
  return DateTime.now().minus({ years: 5 }).toISO();
};
