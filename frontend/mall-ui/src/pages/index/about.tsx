import illustration_avatar from '@/assets/static/illustrations/illustration_avatar.png';
import u from '@/utils';
import { Box, Paper, Typography } from '@mui/material';
import Button from '@mui/material/Button';
import { styled } from '@mui/material/styles';
import { history } from 'umi';

const StyledButton = styled(Button)(({ theme }) => ({
  backgroundColor: 'white',
  borderRadius: 100,
  paddingLeft: 32,
  paddingRight: 32,
  color: 'black',
  textTransform: 'none',
  border: `2px solid ${theme.palette.error.main}`,
  '&:hover': {
    backgroundColor: 'rgba(250,250,250)',
  },
}));

export default function CardReward() {
  return (
    <Paper
      sx={{
        mx: 2,
        mb: 2,
        p: '3px',
        display: 'flex',
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
      }}
      elevation={0}
    >
      <Box
        sx={{
          flexGrow: 1,
          display: 'flex',
          flexDirection: 'row',
          alignItems: 'center',
          justifyContent: 'center',
        }}
      >
        <img
          alt=""
          src={illustration_avatar}
          style={{
            width: '100px',
          }}
        />
      </Box>
      <Box sx={{ flexGrow: 1 }}>
        <Typography
          variant="overline"
          color="text.disabled"
          gutterBottom
          component={'div'}
        >{`${u.config.app.name}`}</Typography>
        <Typography variant={'h6'} gutterBottom component={'div'}>
          🎉新店开业
        </Typography>
        <StyledButton
          size="small"
          onClick={() => {
            history.push({
              pathname: '/about',
            });
          }}
        >
          了解更多
        </StyledButton>
      </Box>
    </Paper>
  );
}
