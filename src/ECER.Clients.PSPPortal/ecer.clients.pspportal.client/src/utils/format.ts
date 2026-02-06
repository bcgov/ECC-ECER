import { DateTime } from "luxon";

export function formatDate(
  inputDate: string | null | undefined,
  toFormat: DateFormat = "yyyy-MM-dd",
) {
  if (inputDate === undefined || inputDate === null) {
    return "Invalid date";
  }
  if (inputDate) {
    const formattedDate = DateTime.fromISO(inputDate).toFormat(toFormat);

    return formattedDate;
  }

  return inputDate;
}
