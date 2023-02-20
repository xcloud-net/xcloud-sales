import u from '@/utils';
import { MallUserDto, SysAdminDto } from '@/utils/models';
import Cookies from 'js-cookie';
import * as React from 'react';

export default () => {
  const [StoreUser, _StoreUser] = React.useState<MallUserDto>({});
  const [StoreUserLoaded, _StoreUserLoaded] = React.useState(false);

  //admin
  const [StoreAdmin, _StoreAdmin] = React.useState<SysAdminDto>({});
  const [StoreAdminLoaded, _StoreAdminLoaded] = React.useState(false);

  const cacheIsValid = (key: string) => {
    if (!u.isEmpty(Cookies.get(key))) {
      return true;
    }
    //two minutes
    var expired = (1 / (24 * 60)) * 2;
    Cookies.set(key, 'true', { expires: expired });

    return false;
  };

  const removeCacheKey = (key: string) => Cookies.remove(key);

  const queryAdminProfile: any = async () => {
    if (!u.hasAccessToken()) {
      _StoreAdmin({});
      _StoreAdminLoaded(true);
      return;
    }
    try {
      var adminResponse = await u.http.adminRequest.post('/admin/profile', {});

      var profile: SysAdminDto = adminResponse.data.Data || {};

      profile.NickName = u.firstNotEmpty([
        profile.NickName,
        profile.SysUser?.NickName,
        '--',
      ]);
      profile.Avatar = u.firstNotEmpty([
        profile.Avatar,
        profile.SysUser?.Avatar,
      ]);

      _StoreAdmin(profile);
    } catch (e) {
      _StoreAdmin({});
    } finally {
      _StoreAdminLoaded(true);
    }
  };

  const queryUserProfile: any = async () => {
    if (!u.hasAccessToken()) {
      _StoreUser({});
      _StoreUserLoaded(true);
      return;
    }
    try {
      var res = await u.http.apiRequest.post('/mall/user/profile');
      var profile: MallUserDto = res.data.Data || {};

      profile.NickName = u.firstNotEmpty([
        profile.NickName,
        profile.SysUser?.NickName,
        '--',
      ]);
      profile.Avatar = u.firstNotEmpty([
        profile.Avatar,
        profile.SysUser?.Avatar,
      ]);
      profile.AccountMobile = u.firstNotEmpty([
        profile.AccountMobile,
        profile.SysUser?.AccountMobile,
      ]);

      _StoreUser(profile);
    } catch (e) {
      _StoreUser({});
    } finally {
      _StoreUserLoaded(true);
    }
  };

  const isAdminLogin = () => StoreAdmin && !u.isEmpty(StoreAdmin.Id);
  const isUserLogin = () => StoreUser && StoreUser.Id && StoreUser.Id > 0;

  return {
    StoreUser,
    _StoreUser,
    StoreUserLoaded,
    _StoreUserLoaded,
    StoreAdmin,
    _StoreAdmin,
    StoreAdminLoaded,
    _StoreAdminLoaded,
    cacheIsValid,
    removeCacheKey,
    queryAdminProfile,
    queryUserProfile,
    isAdminLogin,
    isUserLogin,
  };
};
