import XAreaSelector from '@/components/addressSelector';
import u from '@/utils';
import http from '@/utils/http';
import { LoadingButton } from '@mui/lab';
import {
  Box,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Stack,
  TextField,
} from '@mui/material';
import * as React from 'react';
import LinearProgress from '@/components/loading/linear';

export default function FormDialog(props: any) {
  const { open, onClose, data, onOk, onDelete, onSet } = props;

  const addressTemplate = {
    Id: '',
    Name: '',
    NationCode: '',
    Nation: '',
    ProvinceCode: '',
    Province: '',
    CityCode: '',
    City: '',
    AreaCode: '',
    Area: '',
    AddressDescription: '',
    AddressDetail: '',
    PostalCode: '',
    Tel: '',
    IsDefault: true,
  };

  const [formData, _formData] = React.useState(addressTemplate);
  const [loading, _loading] = React.useState(false);
  const [loadingDelete, _loadingDelete] = React.useState(false);
  const [loadingSet, _loadingSet] = React.useState(false);

  const setAddressAsDefault = (id: string) => {
    _loadingSet(true);
    http.platformRequest
      .post('/user/address/set-default', {
        Id: id,
      })
      .then((res) => {
        if (res.data.Error) {
          alert(res.data.Error.Message);
        } else {
          onSet && onSet();
        }
      })
      .finally(() => {
        _loadingSet(false);
      });
  };

  const deleteAddress = (id: string) => {
    _loadingDelete(true);
    http.platformRequest
      .post('/user/address/delete', {
        Id: id,
      })
      .then((res) => {
        if (res.data.Error) {
          alert(res.data.Error.Message);
        } else {
          onDelete && onDelete();
        }
      })
      .finally(() => {
        _loadingDelete(false);
      });
  };

  const saveAddress = () => {
    if (
      u.isEmpty(formData.Name) ||
      u.isEmpty(formData.Province) ||
      u.isEmpty(formData.City) ||
      u.isEmpty(formData.Area) ||
      u.isEmpty(formData.AddressDetail)
    ) {
      u.error('请完善表单信息');
      return;
    }

    if (!u.validator.isMobile(formData.Tel)) {
      u.error('手机号码格式不对');
      return;
    }

    _loading(true);

    http.platformRequest
      .post('/user/address/save', {
        ...formData,
        IsDefault: true,
      })
      .then((res) => {
        if (res.data.Error) {
          alert(res.data.Error.Message);
        } else {
          onOk && onOk();
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  React.useEffect(() => {
    if (data) {
      console.log(data);
      _formData(data);
    }
  }, [data]);

  return (
    <>
      <Dialog
        fullWidth
        scroll="body"
        open={open}
        onClose={() => {
          onClose && onClose();
        }}
      >
        {loading && <LinearProgress />}
        <DialogTitle>联系地址</DialogTitle>
        <DialogContent>
          <Box sx={{ py: 2 }}>
            <Stack spacing={2}>
              <TextField
                label="姓名"
                fullWidth
                value={formData.Name}
                onChange={(e) => {
                  _formData({ ...formData, Name: e.target.value });
                }}
                variant="outlined"
              />
              <TextField
                label="电话"
                fullWidth
                value={formData.Tel}
                onChange={(e) => {
                  _formData({ ...formData, Tel: e.target.value });
                }}
                variant="outlined"
              />
              <Box sx={{ display: 'none' }}>
                <XAreaSelector
                  selected={[
                    formData.ProvinceCode,
                    formData.CityCode,
                    formData.AreaCode,
                  ]}
                  onProvinceChange={(province: any) => {
                    _formData({ ...formData, ProvinceCode: province });
                  }}
                  onCityChange={(city: any) => {
                    _formData({ ...formData, CityCode: city });
                  }}
                  onAreaChange={(area: any) => {
                    _formData({ ...formData, AreaCode: area });
                  }}
                />
              </Box>
              <TextField
                label="省"
                fullWidth
                value={formData.Province}
                onChange={(e) => {
                  _formData({ ...formData, Province: e.target.value });
                }}
                variant="outlined"
              />
              <TextField
                label="市"
                fullWidth
                value={formData.City}
                onChange={(e) => {
                  _formData({ ...formData, City: e.target.value });
                }}
                variant="outlined"
              />
              <TextField
                label="区县"
                fullWidth
                value={formData.Area}
                onChange={(e) => {
                  _formData({ ...formData, Area: e.target.value });
                }}
                variant="outlined"
              />
              <TextField
                label="详细地址"
                fullWidth
                multiline
                minRows={3}
                value={formData.AddressDetail}
                onChange={(e) => {
                  _formData({ ...formData, AddressDetail: e.target.value });
                }}
                variant="outlined"
              />
            </Stack>
          </Box>
        </DialogContent>
        <DialogActions>
          {u.isEmpty(formData.Id) || (
            <>
              {formData.IsDefault || (
                <LoadingButton
                  loading={loadingSet}
                  onClick={() => {
                    setAddressAsDefault(formData.Id);
                  }}
                >
                  设为默认
                </LoadingButton>
              )}
              <LoadingButton
                loading={loadingDelete}
                color="error"
                onClick={() => {
                  if (confirm('确定删除该地址？')) {
                    deleteAddress(formData.Id);
                  }
                }}
              >
                删除
              </LoadingButton>
            </>
          )}
          <LoadingButton
            loading={loading}
            onClick={() => {
              saveAddress();
            }}
          >
            保存
          </LoadingButton>
        </DialogActions>
      </Dialog>
    </>
  );
}
