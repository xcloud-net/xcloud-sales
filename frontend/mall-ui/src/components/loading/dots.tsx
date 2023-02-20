import { Stack } from '@mui/material';
import { DotLoading } from 'antd-mobile';

export default function VerticalTabs(props: {}) {
  return (
    <>
      <Stack
        spacing={2}
        alignItems="center"
        justifyContent="center"
        direction="row"
      >
        <DotLoading />
      </Stack>
    </>
  );
}
