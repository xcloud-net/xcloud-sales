import u from '@/utils';
import { LoadingButton } from '@mui/lab';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import FormControl from '@mui/material/FormControl';
import FormControlLabel from '@mui/material/FormControlLabel';
import Radio from '@mui/material/Radio';
import RadioGroup from '@mui/material/RadioGroup';
import * as React from 'react';

export default function Animations(props: any) {
  const { model, ok, show, hide } = props;

  const [loadingSave, _loadingSave] = React.useState(false);

  const [loadingBalance, _loadingBalance] = React.useState(false);
  const [balance, _balance] = React.useState({
    Balance: 0,
    Points: 0,
  });

  const balanceEnough = balance.Balance >= model.OrderTotal;

  const queryBalance = () => {
    _loadingBalance(true);
    u.http.apiRequest
      .post('/mall/user/balance-and-points')
      .then((res) => {
        u.handleResponse(res, () => {
          var data = res.data.Data;
          data && _balance(data);
        });
      })
      .finally(() => {
        _loadingBalance(false);
      });
  };

  React.useEffect(() => {
    show && queryBalance();
  }, [show]);

  const paywithbalance = () => {
    _loadingSave(true);
    u.http.apiRequest
      .post('/mall/balance/create-order-payment', {
        Id: model.Id,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          hide && hide();
          ok && ok();
        });
      })
      .finally(() => {
        _loadingSave(false);
      });
  };

  const paywithwechat = () => {
    _loadingSave(true);
    u.http.apiRequest
      .post('/mall/wechat/xxxx', {})
      .then((res) => {
        var error = res.data.Error;
        if (error) {
          alert(error.Message);
        } else {
          ok && ok();
        }
      })
      .finally(() => {
        _loadingSave(false);
      });
  };

  const [selectedPayment, _selectedPayment] = React.useState('');

  const paymentOption = [
    {
      key: 'wechat',
      name: '????????????',
      btn: '????????????',
      disabled: true,
      action: () => {
        paywithwechat();
      },
    },
    {
      key: 'alipay',
      name: '???????????????',
      btn: '???????????????',
      disabled: true,
      action: () => {
        //nothing
      },
    },
    {
      key: 'balance',
      name: loadingBalance
        ? `loading...`
        : balanceEnough
        ? `???????????????${balance.Balance}??????`
        : `???????????????${balance.Balance}?????????????????????`,
      btn: '????????????',
      disabled: !balanceEnough,
      action: () => {
        paywithbalance();
      },
    },
  ];

  const selectedOption = u.find(paymentOption, (x) => x.key == selectedPayment);

  React.useEffect(() => {
    var findedOption = u.find(paymentOption, (x) => !x.disabled);
    if (findedOption) {
      _selectedPayment(findedOption.key);
    }
  }, [balance]);

  return (
    <>
      <Dialog open={show} onClose={() => hide && hide()} fullWidth>
        <DialogTitle>?????????</DialogTitle>
        <DialogContent>
          <DialogContentText>?????????????????????</DialogContentText>
          <FormControl>
            <RadioGroup value={selectedPayment}>
              {u.map(paymentOption, (x, index) => (
                <FormControlLabel
                  value={x.key}
                  disabled={x.disabled}
                  control={<Radio />}
                  label={x.name}
                />
              ))}
            </RadioGroup>
          </FormControl>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => hide && hide()}>??????</Button>
          <LoadingButton
            disabled={!selectedOption}
            onClick={() => {
              selectedOption && selectedOption.action();
            }}
            loading={loadingSave}
          >
            {selectedOption?.btn || '??????'}
          </LoadingButton>
        </DialogActions>
      </Dialog>
    </>
  );
}
