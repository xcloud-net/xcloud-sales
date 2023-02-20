import u from '@/utils';
import http from '@/utils/http';
import {
  GoodsCombinationDto,
  GoodsGradePriceDto,
  UserGradeDto,
} from '@/utils/models';
import { EditOutlined } from '@ant-design/icons';
import {
  Button,
  Checkbox,
  Form,
  InputNumber,
  Modal,
  Space,
  message,
  Tag,
  Tooltip,
} from 'antd';
import { useEffect, useState } from 'react';

interface XProps {
  model: GoodsCombinationDto;
  ok: any;
}

export default (props: XProps) => {
  const { model, ok } = props;
  const [show, _show] = useState(false);
  const [loading, _loading] = useState(false);

  const [formData, _formData] = useState<GoodsCombinationDto>({});
  const [gradeprices, _gradeprices] = useState<GoodsGradePriceDto[]>([]);
  const [gradelist, _gradelist] = useState<UserGradeDto[]>([]);

  const [form] = Form.useForm();

  const renderOffset = (x: GoodsGradePriceDto) => {
    var originPrice = model.Price || 0;
    var finalPrice = x.Price || 0;
    var offset = finalPrice - originPrice;

    if (offset > 0) {
      return `涨价${offset}`;
    } else if (offset < 0) {
      return `降价${offset}`;
    } else {
      return '价格保持不变';
    }
  };

  const queryGradeList = () => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/user-grade/list', {})
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          _gradelist(res.data.Data || []);
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  const saveStatus = () => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/combination/update-price', {
        Id: model.Id,
        ...formData,
        GradePriceToSave: gradeprices || [],
      })
      .then((res) => {
        u.handleResponse(res, () => {
          message.success('修改成功');
          _show(false);
          ok && ok();
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    if (!model) {
      return;
    }
    _formData(model);
    _gradeprices(model.AllGradePrices || []);
  }, [model]);

  useEffect(() => {
    if (show) {
      queryGradeList();
    }
  }, [show]);

  useEffect(() => {
    console.log(gradeprices);
  }, [gradeprices]);

  const renderGradePriceFormRow = (x: UserGradeDto) => {
    var thisPrice = gradeprices.find((d) => d.GradeId == x.Id);
    var otherPrices = gradeprices.filter((d) => d.GradeId != x.Id);
    var checked = thisPrice != null;

    return (
      <>
        <div
          style={{
            display: 'flex',
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'space-between',
            marginBottom: 10,
          }}
        >
          <InputNumber
            disabled={!checked}
            value={thisPrice?.Price}
            onChange={(e) => {
              if (!checked) {
                return;
              }
              if (e == null) {
                _gradeprices(otherPrices);
              } else {
                _gradeprices([
                  ...otherPrices,
                  {
                    ...thisPrice,
                    Price: e || 0,
                  },
                ]);
              }
            }}
          />
          <Checkbox
            checked={checked}
            onChange={(e) => {
              console.log('otherPrices', otherPrices);
              if (e.target.checked) {
                _gradeprices([
                  ...otherPrices,
                  {
                    GoodsCombinationId: model.Id,
                    GradeId: x.Id,
                    Price: model.Price || 0,
                  },
                ]);
              } else {
                _gradeprices(otherPrices);
              }
            }}
          >
            {x.Name}
          </Checkbox>
        </div>
      </>
    );
  };

  return (
    <>
      <Space direction="horizontal">
        <div>
          <p>{`成本价：${model.CostPrice}`}</p>
          <p>{`零售价：${model.Price}`}</p>
          {u.isEmpty(gradeprices) || (
            <div
              style={{
                padding: 3,
                backgroundColor: 'rgb(250,250,250)',
              }}
            >
              {gradeprices.map((x, i) => (
                <p key={i}>
                  <Tooltip title={renderOffset(x)}>
                    <Tag color={'red'}>{`${x.GradeName || '--'}:${
                      x.Price
                    }`}</Tag>
                  </Tooltip>
                </p>
              ))}
            </div>
          )}
        </div>
        <Button
          onClick={() => {
            _show(true);
          }}
          icon={<EditOutlined />}
          size="small"
        ></Button>
      </Space>
      <Modal
        title="设置价格"
        confirmLoading={loading}
        open={show}
        onCancel={() => _show(false)}
        onOk={() => form.submit()}
      >
        <Form
          form={form}
          onFinish={(e) => saveStatus()}
          labelCol={{ flex: '110px' }}
          labelAlign="right"
          wrapperCol={{ flex: 1 }}
        >
          <Form.Item label="成本价">
            <InputNumber
              value={formData.CostPrice}
              onChange={(e) => {
                _formData({
                  ...formData,
                  CostPrice: e || 0,
                });
              }}
            />
          </Form.Item>
          <Form.Item label="零售价">
            <InputNumber
              value={formData.Price}
              onChange={(e) => {
                _formData({
                  ...formData,
                  Price: e || 0,
                });
              }}
            />
          </Form.Item>
          <Form.Item label="会员价">
            <div style={{ marginLeft: 0 }}>
              {u.map(gradelist, (x, index) => (
                <div key={index}>{renderGradePriceFormRow(x)}</div>
              ))}
            </div>
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};
