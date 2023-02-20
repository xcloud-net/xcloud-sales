import LinearProgress from '@mui/material/LinearProgress';
import { styled } from '@mui/material/styles';

const BorderLinearProgress = styled(LinearProgress)(({ theme }) => ({
  height: 1,
  borderRadius: 2,
  backgroundColor: theme.palette.primary.light,
}));

export default () => {
  return <BorderLinearProgress />;
};
