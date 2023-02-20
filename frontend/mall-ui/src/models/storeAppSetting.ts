import * as React from 'react';
import { MallSettingsDto } from '@/utils/models';
import u from '@/utils';

export default () => {
  const [headerHeight, _headerHeight] = React.useState(0);
  const [bottomHeight, _bottomHeight] = React.useState(0);

  const [mallSettings, _mallSettings] = React.useState<MallSettingsDto>({});

  const queryMallSettings = () => {
    u.http.apiRequest.post('/mall/setting/mall-settings', {}).then((res) => {
      u.handleResponse(res, () => {
        _mallSettings(res.data.Data || {});
      });
    });
  };

  return {
    headerHeight,
    _headerHeight,
    bottomHeight,
    _bottomHeight,
    mallSettings,
    queryMallSettings,
  };
};
