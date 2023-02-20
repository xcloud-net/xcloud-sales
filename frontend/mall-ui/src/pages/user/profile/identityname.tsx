import { Typography } from '@mui/material';
import XItem from './item';

export default function Animations(props: any) {
  const { model, ok } = props;

  return (
    <>
      <XItem
        title={'用户名'}
        right={
          <Typography
            variant="overline"
            sx={{
              color: 'text.secondary',
            }}
          >
            {model.IdentityName || '--'}
          </Typography>
        }
      />
    </>
  );
}
