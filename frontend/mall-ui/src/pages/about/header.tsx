import AboutBg from '@/assets/about-bg.webp';
import XLogo from '@/assets/logo-sm.png';
import { Box, Container, Typography, styled } from '@mui/material';
import { Image } from 'antd-mobile';

const XSpan = styled('span')((theme) => ({
  backgroundImage: `linear-gradient(45deg, rgb(230, 73, 128) 0%, rgb(250, 176, 5) 100%)`,
  backgroundClip: 'text',
  '-webkit-tap-highlight-color': 'transparent',
  '-webkit-text-fill-color': 'transparent',
}));

export default function HeroContentLeft() {
  return (
    <>
      <Box
        component="div"
        sx={{
          position: 'relative',
          backgroundImage: `url('${AboutBg}')`,
          backgroundSize: 'cover',
          backgroundPosition: 'center center',
        }}
      >
        <div
          style={{
            position: 'absolute',
            left: 0,
            top: 0,
            right: 0,
            bottom: 0,
            zIndex: 0,
            backgroundImage: `linear-gradient(180deg, rgba(0, 0, 0, 0.25) 0%, rgba(0, 0, 0, .65) 40%)`,
            opacity: 1,
          }}
        ></div>
        <Container
          maxWidth="sm"
          sx={{
            zIndex: 1,
            position: 'relative',
            paddingTop: `70px`,
            paddingBottom: `70px`,
          }}
        >
          <Typography
            variant="h2"
            sx={{
              color: 'white',
            }}
          >
            <XSpan>豆芽家</XSpan>
            <br />
            <span>是一家由</span>
            <XSpan>出入境领队</XSpan>
            <span>创建的</span>
            <XSpan>美妆工作室</XSpan>
          </Typography>

          <Image
            style={{
              width: '100px',
              marginTop: 30,
            }}
            src={XLogo}
            alt=""
          />
        </Container>
      </Box>
    </>
  );
}
