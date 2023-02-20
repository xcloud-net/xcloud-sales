import XPage from '@/components/xpage';
import { Box } from '@mui/material';
import { useEffect, useState } from 'react';
import XLoading from './components/loading';
import u from '@/utils';

export default (props: { data: string; loading: boolean }) => {
  const { data, loading } = props;

  const [jsonData, _jsonData] = useState<any>({});

  useEffect(() => {
    try {
      u.isEmpty(data) || _jsonData(JSON.parse(data));
    } catch (e) {
      console.log(e);
    }
  }, [data]);

  return (
    <>
      {loading && <XLoading />}
      {loading || (
        <Box
          sx={{
            //px: 1,
            mb: 3,
          }}
        >
          <XPage data={jsonData || {}} />
        </Box>
      )}
    </>
  );
};
