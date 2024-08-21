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
  return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + sizes[i];
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
