import u from '@/utils';
import { Alert } from '@mui/material';
import Box from '@mui/material/Box';
import FormControl from '@mui/material/FormControl';
import InputLabel from '@mui/material/InputLabel';
import MenuItem from '@mui/material/MenuItem';
import Select from '@mui/material/Select';
import * as React from 'react';
import { history } from 'umi';
import LinearProgress from '@/components/loading/linear';
import { AddressDto } from '@/utils/models';

export default function BasicSelect(props: any) {
  const { onSelect, onSelectedAddress } = props;

  const [selected, _selected] = React.useState('');
  const [addressList, _addressList] = React.useState<AddressDto[]>([]);
  const [loading, _loading] = React.useState(false);

  const queryList = () => {
    _loading(true);
    u.http.platformRequest
      .post('/user/address/list', {})
      .then((res) => {
        u.handleResponse(res, () => {
          _addressList(res.data.Data || []);
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  React.useEffect(() => {
    queryList();
  }, []);

  React.useEffect(() => {
    u.isEmpty(selected) || (onSelect && onSelect(selected));

    var address = u.find(addressList, (x) => x.Id == selected);
    address && onSelectedAddress && onSelectedAddress(address);
  }, [selected]);

  const convertAddress = (address: AddressDto) => {
    return `${address.Name}@${address.Province}-${address.City}-${address.Area}-${address.AddressDetail}`;
  };

  return (
    <Box sx={{}}>
      {loading && <LinearProgress />}
      {loading || (
        <>
          {u.isEmpty(addressList) && (
            <Alert
              onClick={() => {
                history.push({
                  pathname: '/user/address',
                });
              }}
            >
              你还没有配送地址，点击添加
            </Alert>
          )}
          {u.isEmpty(addressList) || (
            <FormControl fullWidth>
              <InputLabel id='demo-simple-select-label'>配送地址</InputLabel>
              <Select
                labelId='demo-simple-select-label'
                value={selected}
                label='配送地址'
                placeholder='请选择配送地址'
                onChange={(e) => {
                  _selected(e.target.value);
                }}
              >
                {u.map(addressList, (x, index) => (
                  <MenuItem key={x.Id} value={x.Id}>
                    {convertAddress(x)}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          )}
        </>
      )}
    </Box>
  );
}
