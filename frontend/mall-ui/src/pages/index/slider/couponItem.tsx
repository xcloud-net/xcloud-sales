import bow_transparent from '@/assets/bow-transparent.png';
import u from '@/utils';
import { CouponDto } from '@/utils/models';
import { LoadingButton } from '@mui/lab';
import { Box, Typography } from '@mui/material';
import Card from '@mui/material/Card';
import { styled } from '@mui/material/styles';
import { useState } from 'react';

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

const ButtonLearnMore = styled(LoadingButton)(() => ({
  backgroundColor: '#fff !important',
  color: '#fb703c',
  boxShadow: '0 2px 6px #d0efef',
  borderRadius: 12,
  minWidth: 120,
  minHeight: 4,
  textTransform: 'initial',
  fontSize: '0.875rem',
  fontWeight: 700,
  letterSpacing: 0,
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

export default function CardOffer({
  model,
  ok,
}: {
  model: CouponDto;
  ok: any;
}) {
  const [loading, _loading] = useState(false);

  const issueCoupon = () => {
    _loading(true);
    u.http.apiRequest
      .post('/mall/coupon/issue-coupon', {
        CouponId: model.Id,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          u.success('领取成功');
          ok && ok();
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

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
            {model.Title || '🈵️满100减8元优惠券'}
          </Typography>
          {u.isEmpty(model.EndTime) || (
            <Typography
              sx={{
                color: '#48bbb5',
                fontSize: '0.875rem',
                fontWeight: 500,
              }}
            >
              过期时间：{u.dateTimeFromNow(model.EndTime || '')}
            </Typography>
          )}
        </Box>
        <Box sx={{}}>
          <ButtonLearnMore
            disabled={!model.CanBeIssued}
            onClick={() => {
              issueCoupon();
            }}
            loading={loading}
          >
            {model.CanBeIssued ? '领取' : '已领取'}
          </ButtonLearnMore>
        </Box>
      </Box>
      <StyledImg alt={''} src={bow_transparent} />
      <StyledDiv />
    </StyledCard>
  );
}
