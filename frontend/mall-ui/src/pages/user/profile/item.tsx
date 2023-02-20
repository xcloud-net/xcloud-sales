import { Box, Divider, Typography, CardActionArea } from '@mui/material';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';

export default function Animations(props: any) {
  const { title, right } = props;
  return (
    <>
      <CardActionArea sx={{}}>
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'flex-end',
            p: 2,
          }}
        >
          <Box sx={{}} flexGrow={1}>
            <Typography variant="button">{title || '--'}</Typography>
          </Box>
          <Box sx={{}}>{right}</Box>
          <ChevronRightIcon sx={{ color: 'gray', ml: 1 }} />
        </Box>
        <Divider />
      </CardActionArea>
    </>
  );
}
