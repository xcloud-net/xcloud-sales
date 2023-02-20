import { Box } from '@mui/material';
import XBlocks from './blocks';

export default function IndexPage(props: { data?: any }) {
  const { data } = props;

  if (!data) {
    return <></>;
  }

  return (
    <>
      <Box sx={{}}>
        <XBlocks data={data.items} />
      </Box>
    </>
  );
}
