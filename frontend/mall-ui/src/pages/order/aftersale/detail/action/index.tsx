import { Box, Button, ButtonGroup } from '@mui/material';
import { useState } from 'react';
import XCancel from './cancel';
import XComplete from './complete';
import { AfterSaleDto, AfterSalesItemDto } from '@/utils/models';

export default (props: { model: AfterSaleDto; ok: any }) => {
  const { model, ok } = props;

  const { AfterSalesStatusId } = model;

  const [showcancel, _showcancel] = useState(false);
  const [showcomplete, _showcomplete] = useState(false);

  const triggerReload = () => ok && ok();
  return (
    <>
      <XCancel
        model={model}
        show={showcancel}
        hide={() => _showcancel(false)}
        ok={() => triggerReload()}
      />
      <XComplete
        model={model}
        show={showcomplete}
        hide={() => _showcomplete(false)}
        ok={() => triggerReload()}
      />
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'row',
          alignItems: 'center',
          justifyContent: 'center',
          px: 3,
        }}
      >
        <ButtonGroup sx={{ display: 'none' }} size={'large'} fullWidth>
          <Button
            color={'error'}
            onClick={() => {
              _showcancel(true);
            }}
          >
            取消
          </Button>
          <Button
            color={'primary'}
            onClick={() => {
              _showcomplete(true);
            }}
          >
            完成
          </Button>
        </ButtonGroup>
      </Box>
    </>
  );
};
