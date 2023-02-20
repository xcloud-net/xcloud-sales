import bow_transparent from '@/assets/bow-transparent.png';
import u from '@/utils';
import { UserCouponDto } from '@/utils/models';
import { Box, Typography } from '@mui/material';
import Card from '@mui/material/Card';
import { styled } from '@mui/material/styles';

const StyledCard = styled(Card)(() => ({
  position: 'relative',
  borderRadius: 16,
  padding: 12,
  backgroundColor: '#e5fcfb',
  minWidth: 300,
  boxShadow: '0 0 20px 0 rgba(0,0,0,0.12)',
  transition: '0.3s',
  '&:hover': {
    transform: 'translateY(-3px)',
    boxShadow: '0 4px 20px 0 rgba(0,0,0,0.12)',
  },
}));

const StyledImg = styled('img')(() => ({
  position: 'absolute',
  width: '40%',
  bottom: 0,
  right: 0,
  display: 'block',
}));

const StyledDiv = styled('div')(() => ({
  position: 'absolute',
  bottom: 0,
  right: 0,
  transform: 'translate(70%, 50%)',
  borderRadius: '50%',
  backgroundColor: 'rgba(71, 167, 162, 0.12)',
  padding: '40%',

  '&:before': {
    position: 'absolute',
    borderRadius: '50%',
    content: '""',
    display: 'block',
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    margin: '-16%',
    backgroundColor: 'rgba(71, 167, 162, 0.08)',
  },
}));

export default function CardOffer({ model }: { model: UserCouponDto }) {
  return (
    <StyledCard>
      <Box sx={{ mr: 2 }}>
        <Box sx={{ mb: 3 }}>
          <Typography
            sx={{
              color: '#fb703c',
              fontSize: '1.125rem',
              fontWeight: 700,
              lineHeight: 1.6,
              letterSpacing: '0.0075em',
            }}
            gutterBottom
          >
            {`优惠金额：${model.Value || 0}`}
          </Typography>
          {model.MinimumConsumption && (
            <Typography
              sx={{
                color: '#48bbb5',
                fontSize: '0.875rem',
                fontWeight: 500,
              }}
              gutterBottom
            >
              {`最低消费${model.MinimumConsumption || 0}元`}
            </Typography>
          )}
          {u.isEmpty(model.ExpiredAt) || (
            <Typography
              sx={{
                color: '#48bbb5',
                fontSize: '0.875rem',
                fontWeight: 500,
              }}
              gutterBottom
            >
              过期时间：{u.dateTimeFromNowV1(model.ExpiredAt || '')}
            </Typography>
          )}
        </Box>
      </Box>
      <StyledImg alt={''} src={bow_transparent} />
      <StyledDiv />
    </StyledCard>
  );
}
