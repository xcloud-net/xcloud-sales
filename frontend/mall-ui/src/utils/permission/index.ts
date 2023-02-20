import data from './data';

const all = data.groups.flatMap((x) => x.permissions);

export default {
  data,
  all,
};
