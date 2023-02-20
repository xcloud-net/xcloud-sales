import { Card, CardActionArea, Typography } from '@mui/material';
import { alpha, styled } from '@mui/material/styles';
import { history } from 'umi';

const IconWrapperStyle = styled('div')(({ theme }) => ({
  margin: 'auto',
  display: 'flex',
  borderRadius: '50%',
  alignItems: 'center',
  width: theme.spacing(8),
  height: theme.spacing(8),
  justifyContent: 'center',
  marginBottom: theme.spacing(3),
}));

export default function AppWidgetSummary({
  title,
  desc,
  icon,
  color = 'primary',
  sx,
  path,
  onClick,
  ...other
}: any) {
  return (
    <Card
      onClick={() => {
        if (path) {
          history.push(path);
        } else {
          onClick && onClick();
        }
      }}
      {...other}
    >
      <CardActionArea
        sx={{
          py: {
            xs: 3,
            sm: 4,
            md: 5,
            lg: 5,
          },
          boxShadow: 0,
          textAlign: 'center',
          color: (theme: any) => theme.palette[color].darker,
          bgcolor: (theme: any) => theme.palette[color].lighter,
          ...sx,
        }}
      >
        <IconWrapperStyle
          sx={{
            color: (theme: any) => theme.palette[color].dark,
            backgroundImage: (theme: any) =>
              `linear-gradient(135deg, ${alpha(
                theme.palette[color].dark,
                0,
              )} 0%, ${alpha(theme.palette[color].dark, 0.24)} 100%)`,
          }}
        >
          {icon}
        </IconWrapperStyle>

        <Typography variant="h5">{title}</Typography>
        <Typography variant="subtitle2" sx={{ opacity: 0.72 }}>
          {desc}
        </Typography>
      </CardActionArea>
    </Card>
  );
}
