import u from '@/utils';

const querySysUserList = async (ids: any) => {
  if (u.isEmpty(ids)) {
    return [];
  }

  var res = await u.http.apiRequest.post('/sys/user/users-by-ids', ids);

  if (!u.handleResponse(res, () => {})) {
    throw res;
  }

  return res.data.Data || [];
};
export default {
  querySysUserList,
};
