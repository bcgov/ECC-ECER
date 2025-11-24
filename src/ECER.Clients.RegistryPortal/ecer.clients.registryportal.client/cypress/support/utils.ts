// This file contains utility functions and constants for Cypress tests.
// It includes a function to format dates and constants for course start and end dates.
import { DateTime } from "luxon";

/**
 * Course Start Date ISO date string for today minus 200 days.
 */
export const courseStartDay = DateTime.local().minus({ days: 200 }).toFormat("MM/dd/yyyy");

/**
 * Course End Date ISO date string for today minus 50 days.
 */
export const courseEndDay = DateTime.local().minus({ days: 50 }).toFormat("MM/dd/yyyy");

/**
 * Today's day of the month (e.g., 11 for June 11)
 */
export const todayDay = DateTime.local().day;
