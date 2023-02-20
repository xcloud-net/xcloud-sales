import u from '@/utils';
import http from '@/utils/http';
import { SettingsRounded } from '@mui/icons-material';
import AddIcon from '@mui/icons-material/Add';
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  IconButton,
  Typography,
} from '@mui/material';
import * as React from 'react';
import XForm from './form';
import LinearProgress from '@/components/loading/linear';

export default function BasicCard() {
  const [loading, _loading] = React.useState(false);
  const [addressList, _addressList] = React.useState<any[]>([]);

  const [openForm, _openForm] = React.useState(false);
  const [formData, _formData] = React.useState({});

  const getAddressList = () => {
    _loading(true);
    http.platformRequest
      .post('/user/address/list', {})
      .then((res) => {
        var data = u
          .sortBy(res.data.Data || [], (x) => (x.IsDefault ? 1 : 0))
          .reverse();
        _addressList(data);
      })
      .finally(() => {
        _loading(false);
      });
  };

  React.useEffect(() => {
    getAddressList();
  }, []);

  const renderItem = (item: any) => {
    return (
      <>
        <Card sx={{ minWidth: 275, mb: 2 }} onClick={() => {}}>
          <CardContent>
            <Box
              sx={{
                display: 'flex',
                flexDirection: 'row',
                alignItems: 'center',
                justifyContent: 'space-between',
                mb: 1,
              }}
            >
              <Typography variant="h5">{item.Name}</Typography>
              <Typography variant="body2" color="primary">
                {item.Tel}
              </Typography>
            </Box>
            <Typography variant="h5" component="div" gutterBottom>
              {item.Province || '--'}-{item.City || '--'}-{item.Area || '--'}
            </Typography>
            <Typography sx={{ mb: 1.5 }} color="text.secondary" gutterBottom>
              {item.AddressDetail}
            </Typography>
            <Box
              sx={{
                display: 'flex',
                flexDirection: 'row',
                alignItems: 'center',
                justifyContent: 'space-between',
              }}
            >
              <Box>{item.IsDefault && <Chip label="默认" size="small" />}</Box>
              <IconButton
                onClick={() => {
                  _formData(item);
                  _openForm(true);
                }}
              >
                <SettingsRounded />
              </IconButton>
            </Box>
          </CardContent>
        </Card>
      </>
    );
  };

  return (
    <>
      <XForm
        data={formData}
        open={openForm}
        onClose={() => {
          _openForm(false);
        }}
        onOk={() => {
          _openForm(false);
          getAddressList();
        }}
        onDelete={() => {
          _openForm(false);
          getAddressList();
        }}
        onSet={() => {
          _openForm(false);
          getAddressList();
        }}
      />
      {loading && <LinearProgress />}
      <Box sx={{ mx: 1 }}>
        <Box
          sx={{
            mb: 2,
            display: 'flex',
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'flex-end',
          }}
        >
          <Button
            startIcon={<AddIcon />}
            variant="contained"
            onClick={() => {
              _openForm(true);
              _formData({});
            }}
          >
            添加新地址
          </Button>
        </Box>
        {u.map(addressList || [], (x, index) => (
          <Box key={index}>{renderItem(x)}</Box>
        ))}
        {u.isEmpty(addressList) && (
          <Alert severity="success">
            <Typography variant="h6">暂无地址</Typography>
          </Alert>
        )}
      </Box>
    </>
  );
}
