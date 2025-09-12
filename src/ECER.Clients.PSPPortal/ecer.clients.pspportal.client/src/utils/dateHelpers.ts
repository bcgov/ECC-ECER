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
 * Get a date that is a given number of years from today
 * @param years Number of years to add
 */
export const getDatePlusYears = (years: number): string => {
  return DateTime.now().plus({ years }).toISO();
};

/**
 * Get a date that is a given number of years ago from today
 * @param years Number of years to subtract
 */
export const getDateMinusYears = (years: number): string => {
  return DateTime.now().minus({ years }).toISO();
};
