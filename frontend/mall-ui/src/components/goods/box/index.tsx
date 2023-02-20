import { Paper } from '@mui/material';
import XBase, { XGoodsBoxProp } from './base';

const index = function (props: { data: XGoodsBoxProp }) {
  const { model } = props.data;

  if (!model || !model.Id) {
    return null;
  }

  return (
    <>
      <Paper
        sx={{
          overflow: 'hidden',
          borderRadius: '12px',
          cursor: 'pointer',
          '&:active': {
            border: (theme) => `1px solid ${theme.palette.primary.main}`,
          },
        }}
        elevation={1}
      >
        <XBase data={props.data} />
      </Paper>
    </>
  );
};

export default index;
