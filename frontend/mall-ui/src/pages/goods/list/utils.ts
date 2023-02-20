import { decode, encode } from 'js-base64';
import qs from 'qs';
import u from '@/utils';
import { SearchGoodsInputDto, GoodsDto } from '@/utils/models';

const updateUrl = (q: SearchGoodsInputDto) => {
  const { origin, pathname } = window.location;
  var queryString = qs.stringify(
    {
      filter: encode(JSON.stringify(q)),
    },
    {
      skipNulls: true,
    },
  );
  var url = `${origin}${pathname}?${queryString}`;
  window.history.replaceState({}, document.title, url);
};

const buildQueryFromUrl = (query?: any) => {
  var p = null;

  const { kwd, tag, cat, brand, filter } = query || {};

  try {
    if (filter) {
      p = JSON.parse(decode(filter as string));
    }
  } catch (e) {
    console.log(e);
  }

  p = p || {};
  if (kwd) {
    p.Keywords = kwd;
  }
  if (tag) {
    p.TagId = tag;
  }
  if (cat) {
    try {
      p.CategoryId = parseInt(cat);
    } catch (e) {
      console.log(e);
    }
  }
  if (brand) {
    try {
      p.BrandId = parseInt(brand);
    } catch (e) {
      console.log(e);
    }
  }
  p.Page = p.Page || 1;
  return p;
};

const queryParamsFinglePrint = (xx: any) => {
  var str = qs.stringify(xx, {
    sort: (a, b) => {
      return u.toString(a.key).localeCompare(u.toString(b.key));
    },
  });
  return str;
};

export default {
  updateUrl,
  buildQueryFromUrl,
  queryParamsFinglePrint,
};
