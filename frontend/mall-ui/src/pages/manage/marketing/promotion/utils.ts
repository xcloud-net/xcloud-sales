const parseArray = (d: any): any[] => {
  try {
    return JSON.parse(d);
  } catch (e) {
    return [];
  }
};

export default {
  parseArray,
};
