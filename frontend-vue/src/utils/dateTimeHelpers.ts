const minutes = 15;
const ms = 1000 * 60 * minutes;

/**
 * Credit: https://bobbyhadz.com/blog/javascript-round-time-to-nearest-15-minutes
 * @param {Date?} date
 * @returns Date instance rounded to nearest 15 minutes. Rounds to future or past, whichever is closer.
 */
export function useRoundToNearest15Minutes(date = new Date()) {
  // 👇️ replace Math.round with Math.ceil to always round UP
  return new Date(Math.round(date.getTime() / ms) * ms);
}

/**
 * Credit: https://bobbyhadz.com/blog/javascript-round-time-to-nearest-15-minutes
 * @param {Date?} date
 * @returns Date instance rounded to next 15 minutes. Rounds only to future.
 */
export function useRoundToNext15Minutes(date = new Date()) {
  return new Date(Math.ceil(date.getTime() / ms) * ms);
}

/**
 * Credit: https://bobbyhadz.com/blog/javascript-round-time-to-nearest-15-minutes
 * @param {Date?} date
 * @returns Date instance rounded to previous 15 minutes. Rounds only to past.
 */
export function useRoundToPrevious15Minutes(date = new Date()) {
  return new Date(Math.floor(date.getTime() / ms) * ms);
}
