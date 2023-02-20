import u from '@/utils';
import {
  PromotionConditionDto,
  PromotionDto,
  PromotionResultDto,
} from '@/utils/models';
import { Button, Card, Col, Row } from 'antd';
import { useEffect, useState } from 'react';
import utils from '../../utils';
import XCondition from './condition';
import XResult from './result';

export default (props: { model: PromotionDto; ok: any }) => {
  const { model, ok } = props;

  const [conditionList, _conditionList] = useState<PromotionConditionDto[]>([]);
  const [resultList, _resultList] = useState<PromotionResultDto[]>([]);

  const [show, _show] = useState(false);
  const [loading, _loading] = useState(false);

  const setRules = () => {
    _conditionList(utils.parseArray(model.Condition));
    _resultList(utils.parseArray(model.Result));
  };

  useEffect(() => {
    show && setRules();
  }, [show]);

  useEffect(() => {
    setRules();
  }, [model]);

  const saveRules = () => {
    _loading(true);
    model.PromotionConditions = conditionList;
    model.PromotionResults = resultList;
    u.http.apiRequest
      .post('/mall-admin/promotion/update-rules', {
        ...model,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          ok && ok();
          _show(false);
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  return (
    <>
      <Card
        title="优惠规则"
        loading={loading}
        extra={
          <Button
            onClick={() => {
              saveRules();
            }}
          >
            保存规则
          </Button>
        }
      >
        <Row gutter={10}>
          <Col span={12}>
            <XCondition
              data={conditionList}
              ok={(e: any) => {
                _conditionList([...conditionList, e]);
              }}
              remove={(e: number) => {
                if (!confirm('确定删除规则？')) {
                  return;
                }
                var conditions = conditionList.filter((x, i) => i != e);
                _conditionList(conditions);
              }}
            />
          </Col>
          <Col span={12}>
            <XResult
              data={resultList}
              ok={(e: any) => {
                _resultList([...resultList, e]);
              }}
              remove={(e: number) => {
                if (!confirm('确定删除规则？')) {
                  return;
                }
                var results = resultList.filter((x, i) => i != e);
                _resultList(results);
              }}
            />
          </Col>
        </Row>
      </Card>
    </>
  );
};
