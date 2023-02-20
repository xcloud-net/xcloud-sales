import menus from './data';
import u from '@/utils';
import { IRoute } from '@/utils/models';

const checkIsSelected = (m: IRoute, pathname: string, callback: any) => {
  var path_equal = u.pathEqual(m.path || '', pathname);
  var any_children_path_equal = false;
  if (!u.isEmpty(m.routes)) {
    var children = m.routes || [];
    for (var i = 0; i < children.length; ++i) {
      if (checkIsSelected(children[i], pathname, callback)) {
        any_children_path_equal = true;
        break;
      }
    }
  }

  var finalEqual = path_equal || any_children_path_equal;

  callback && callback(finalEqual, m);

  return finalEqual;
};

const resolveSelectedMenu = (pathname: string) => {
  var path_list: string[] = [];

  u.map(menus, (x) =>
    checkIsSelected(x, pathname, (yes: boolean, r: IRoute) => {
      yes && path_list.push(r.path || '');
    }),
  );

  return path_list;
};

export default {
  checkIsSelected,
  resolveSelectedMenu,
};
