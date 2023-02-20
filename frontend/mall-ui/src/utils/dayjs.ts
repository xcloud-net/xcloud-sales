import dayjs from 'dayjs';
import _ from './lodash';

import _relativeTime from 'dayjs/plugin/relativeTime';
import localizedFormat from 'dayjs/plugin/localizedFormat';
import isToday from 'dayjs/plugin/isToday';
import isYesterday from 'dayjs/plugin/isYesterday';
import isTomorrow from 'dayjs/plugin/isTomorrow';
import utc from 'dayjs/plugin/utc';
import localeData from 'dayjs/plugin/localeData';

import 'dayjs/locale/zh-cn';

dayjs.extend(localeData);
dayjs.extend(_relativeTime);
dayjs.extend(localizedFormat);
dayjs.extend(isToday);
dayjs.extend(isYesterday);
dayjs.extend(isTomorrow);
dayjs.extend(utc);
dayjs.locale('zh-cn');

const timezoneOffset = 8;
const dateFormat = 'YYYY-MM-DD';
const timeFormat = 'YYYY-MM-DD HH:mm:ss';

const getDate = (dateStr: string) => {
  if (_.isEmpty(dateStr)) {
    return null;
  }
  return dayjs(dateStr).add(timezoneOffset, 'hour').format(dateFormat);
};

const dateTimeFromNowV1 = (dateStr: string) => {
  if (_.isEmpty(dateStr)) {
    return null;
  }

  return dayjs(dateStr).add(timezoneOffset, 'hour').fromNow();
};

const dateTimeFromNow = (dateStr: string) => {
  if (_.isEmpty(dateStr)) {
    return null;
  }

  if (Math.abs(dayjs(dateStr).diff(dayjs.utc(), 'day')) > 7) {
    return formatDateTime(dateStr);
  }

  return dayjs(dateStr).add(timezoneOffset, 'hour').fromNow();
};

const formatDateTime = (dateStr: string) => {
  if (_.isEmpty(dateStr)) {
    return null;
  }

  return dayjs(dateStr).add(timezoneOffset, 'hour').format(timeFormat);
};

const formatAsUtcDateTime = (dateStr: string) => {
  if (_.isEmpty(dateStr)) {
    return null;
  }

  return dayjs(dateStr).add(-timezoneOffset, 'hour').format(timeFormat);
};

const getTimeStamp = (dateStr: string) => {
  if (_.isEmpty(dateStr)) {
    return null;
  }
  return dayjs(dateStr).add(timezoneOffset, 'hour').valueOf();
};

const now = () => {
  return dayjs.utc().add(timezoneOffset, 'hour').format(timeFormat);
};

export default {
  timezoneOffset,
  dateFormat,
  timeFormat,
  dayjs,
  now,
  getTimeStamp,
  getDate,
  dateTimeFromNow,
  formatDateTime,
  formatAsUtcDateTime,
  dateTimeFromNowV1,
};
