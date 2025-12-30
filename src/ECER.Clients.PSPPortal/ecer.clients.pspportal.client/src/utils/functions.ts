export function cleanPreferredName(firstName: string | null | undefined, lastName: string | null | undefined, mode = "full") {
  const clean = (str: any) => (str ?? "").trim(); // null/undefined → '' and trim

  const fn = clean(firstName);
  const ln = clean(lastName);

  if (mode === "full") {
    return `${fn} ${ln}`.trim();
  }
  return fn || ln;
}

export function areObjectsEqual(obj1: any, obj2: any): boolean {
  if (obj1 === obj2) {
    return true;
  }

  if (typeof obj1 !== "object" || obj1 === null || typeof obj2 !== "object" || obj2 === null) {
    return false;
  }

  const keys1 = Object.keys(obj1);
  const keys2 = Object.keys(obj2);

  if (keys1.length !== keys2.length) {
    return false;
  }

  for (const key of keys1) {
    if (!keys2.includes(key) || !areObjectsEqual(obj1[key], obj2[key])) {
      return false;
    }
  }

  return true;
}

/**
 * Converting bytes to human readable values (KB, MB, GB, TB, PB, EB, ZB, YB)
 * @param {*} bytes
 * @param {*} decimals
 */

export function humanFileSize(bytes: number, decimals = 2) {
  if (bytes === 0) return "0 Bytes";
  const k = 1024;
  const dm = decimals < 0 ? 0 : decimals;
  const sizes = ["B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + (sizes[i] || "Bigger than YB");
}

/**
 * Converts a human-readable file size string back to bytes.
 * @param {string} humanSize - The human-readable file size (e.g., "1.45 MB").
 * @returns {number} - The size in bytes.
 */
export function parseHumanFileSize(humanSize: string): number {
  // Trim the input and split into size and unit
  const sizePattern = /^(\d+(\.\d+)?)\s?([a-zA-Z]+)$/;
  const match = humanSize.trim().match(sizePattern);

  if (!match) {
    throw new Error("Invalid file size format");
  }

  const size = parseFloat(match[1] || ""); // Convert the number part to a float
  const unit = (match[3] || "").toUpperCase(); // Get the unit part and convert to uppercase for consistency

  // Define units and their corresponding multiplier in bytes
  const units: { [key: string]: number } = {
    B: 1,
    KB: 1024,
    MB: 1024 ** 2,
    GB: 1024 ** 3,
    TB: 1024 ** 4,
    PB: 1024 ** 5,
    EB: 1024 ** 6,
    ZB: 1024 ** 7,
    YB: 1024 ** 8,
  };

  const multiplier = units[unit];
  if (multiplier === undefined) {
    throw new Error("Invalid file size unit");
  }

  return size * multiplier;
}

/**
 * Sorts an array of objects by a specified key, while keeping exceptions at the end.
 * @param {Array<Object>} a - The first object to compare.
 * @param {Array<Object>} b - The second object to compare.
 * @param {string} key - The key by which the objects should be sorted.
 * @param {Array<string>} [exceptions=[]] - An array of strings representing exceptions that should be kept at the end of the sorted array.
 * @returns {number} A negative value if a should come before b, a positive value if a should come after b, or 0 if they are equal.
 */
export function sortArray(a: any, b: any, key: string, exceptions: string[] = []) {
  if (exceptions.includes(a[key])) {
    return 1;
  }

  if (a[key] < b[key]) {
    return -1;
  }

  if (a[key] > b[key]) {
    return 1;
  }

  return 0;
}

/**
 * Removes an element from an array by its index.
 *
 * @param {any[]} array - The array to modify.
 * @param {number} index - The index of the element to remove.
 * @returns {any[]} - A new array with the element removed.
 *
 * @returns {any[]} Original array if the index is invalid.
 */
export function removeElementByIndex(array: any[], index: number) {
  if (index >= 0 && index < array.length) {
    const arrayCopy = array.slice();
    arrayCopy.splice(index, 1);
    return arrayCopy;
  } else {
    console.error("removeElementByIndex() :: invalid index element not removed");
    return array;
  }
}

/**
 * Replaces an element in an array by its index.
 *
 * @param {any[]} array - The array to modify.
 * @param {number} index - The index of the element to replace.
 * @param {any} element - The new element to insert.
 * @returns {any[]} - A new array with the element replaced.
 *
 * @returns {any[]} Original array if the index is invalid.
 */
export function replaceElementByIndex(array: any[], index: number, element: any) {
  if (index >= 0 && index < array.length) {
    const arrayCopy = array.slice();
    arrayCopy.splice(index, 1, element);
    return arrayCopy;
  } else {
    console.error("replaceElementByIndex() :: invalid index element not replaced");
    return array;
  }
}

/**
 * Sanitize file name by removing special (non-ASCII) characters, spaces, dashes, periods,
 * and converting macOS-specific characters to be Windows-compatible.
 * @param {string} fileName
 * @returns {string} Sanitized file name
 */
export function sanitizeFilename(filename: string) {
  // Remove all non-ASCII characters
  // eslint-disable-next-line no-control-regex
  return filename.replace(/[^\x00-\x7F]/g, "");
}

/**
 * Scrolls to a given element using specified options for behavior, block, and inline alignment.
 *
 * @param {HTMLElement} element - The element to scroll to.
 * @param {ScrollBehavior} [behavior='smooth'] - The scroll behavior ('auto', 'smooth').
 * @param {ScrollLogicalPosition} [block='start'] - The vertical alignment ('start', 'center', 'end', 'nearest').
 * @param {ScrollLogicalPosition} [inline='nearest'] - The horizontal alignment ('start', 'center', 'end', 'nearest').
 */
export function scrollToElement(
  element: HTMLElement | null,
  behavior: ScrollBehavior = "smooth",
  block: ScrollLogicalPosition = "start",
  inline: ScrollLogicalPosition = "nearest",
) {
  if (!element) {
    console.error("scrollToElement() :: element not found");
    return;
  }

  element.scrollIntoView({ behavior, block, inline });
}

/**
 * Parses a full name string into first and last name components.
 *
 * This function handles various name formats, including single names,
 * names with multiple first names, and names with leading/trailing spaces.
 * It treats a single name as the last name. Empty or null input will
 * return empty strings for both first and last name.
 *
 * @param {string} name The full name string to parse.
 * @returns {{ firstName: string; lastName: string }} An object containing the parsed first and last names.
 *         If the input is null, undefined, or an empty string, both firstName and lastName will be empty strings.
 *         If the input string contains only spaces, both firstName and lastName will be empty strings.
 */
export function parseFirstNameLastName(name: string) {
  let firstName = "";
  let lastName = "";
  if (!name) {
    return { firstName, lastName };
  }

  name = name.trim();

  let nameArray = name.split(" ");
  if (nameArray.length === 1) {
    lastName = nameArray[0] || "";
  } else if (nameArray.length > 1) {
    lastName = nameArray.pop() || "";
    firstName = nameArray.join(" ");
  }

  return { firstName, lastName };
}

export enum CertificationType {
  Assistant = "Assistant",
  OneYear = "One Year",
  FiveYearCertificate = "Five Year Certificate",
  FiveYearCertificateITE_SNE = "Five Year Certificate+ITE+SNE",
}

const certificationTypeMap: Record<string, CertificationType> = Object.values(CertificationType).reduce(
  (m, v) => {
    m[v] = v;
    return m;
  },
  {} as Record<string, CertificationType>,
);

const certificationWeights: Record<CertificationType, number> = {
  [CertificationType.Assistant]: 1,
  [CertificationType.OneYear]: 2,
  [CertificationType.FiveYearCertificate]: 3,
  [CertificationType.FiveYearCertificateITE_SNE]: 4,
};

export function parseCertificationType(input: string): CertificationType {
  const cert = certificationTypeMap[input];
  if (!cert) throw new Error(`Unrecognized certification type: "${input}"`);
  return cert;
}
