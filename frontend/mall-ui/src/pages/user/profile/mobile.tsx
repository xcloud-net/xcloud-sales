import { Typography } from '@mui/material';
import XItem from './item';

export default function Animations(props: any) {
  const { model, ok } = props;

  return (
    <>
      <XItem
        title={'手机号'}
        right={
          <Typography
            variant="overline"
            sx={{
              color: 'text.secondary',
            }}
          >
            {model.AccountMobile || '--'}
          </Typography>
        }
      />
    </>
  );
}
