import u from '@/utils';
import { Button, Card, Col, Empty, Row } from 'antd';

const App = () => (
  <Row
    justify="center"
    style={{
      marginTop: '100px',
    }}
  >
    <Col span={12}>
      <Card title="提示">
        <Empty
          image="https://gw.alipayobjects.com/zos/antfincdn/ZHrcdLPrvN/empty.svg"
          imageStyle={{
            height: 60,
          }}
          description={
            <>
              <div>
                <h3>登陆过期</h3>
              </div>
            </>
          }
        >
          <Button
            type="primary"
            onClick={() => {
              u.redirectToLogin();
            }}
          >
            立即登录
          </Button>
        </Empty>
      </Card>
    </Col>
  </Row>
);

export default App;
