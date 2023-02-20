import { Box } from '@mui/material';
import u from '@/utils';

export default ({ value }: { value: number }) => {
  const str = u.split(parseFloat(value.toString()).toFixed(2).toString(), '.');

  const left = str[0];
  const right = str[1].padStart(2, '0');

  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'row',
        alignItems: 'flex-start',
        justifyContent: 'flex-start',
        fontStyle: 'normal',
      }}
    >
      <span
        style={{
          fontSize: '14px',
          fontWeight: 'bolder',
          lineHeight: 1.3,
          marginRight: '2px',
        }}
      >
        ¥
      </span>
      <span
        style={{
          fontSize: '23px',
          fontWeight: 'bolder',
          lineHeight: 1,
        }}
      >
        {left}
      </span>
      <span
        style={{
          fontSize: '14px',
          fontWeight: 'bolder',
          lineHeight: 1.3,
        }}
      >
        {`.${right}`}
      </span>
    </Box>
  );
};
