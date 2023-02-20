import { Box } from '@mui/material';
import XTopAppBar from './appBar';

const index = function (props: any) {
  const { children } = props;

  return (
    <>
      <XTopAppBar />
      <Box
        component={'div'}
        sx={(theme) => ({
          paddingBottom: 1,
        })}
      >
        {children}
      </Box>
    </>
  );
};

export default index;
