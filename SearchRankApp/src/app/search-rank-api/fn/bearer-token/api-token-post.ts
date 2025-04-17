/* tslint:disable */
/* eslint-disable */
/* Code generated by ng-openapi-gen DO NOT EDIT. */

import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { LoginRequest as SearchRankPresentationRequestsLoginRequest } from '../../models/SearchRank/Presentation/Requests/login-request';
import { LoginResponse as SearchRankPresentationResponsesLoginResponse } from '../../models/SearchRank/Presentation/Responses/login-response';

export interface ApiTokenPost$Params {
      body: SearchRankPresentationRequestsLoginRequest
}

export function apiTokenPost(http: HttpClient, rootUrl: string, params: ApiTokenPost$Params, context?: HttpContext): Observable<StrictHttpResponse<SearchRankPresentationResponsesLoginResponse>> {
  const rb = new RequestBuilder(rootUrl, apiTokenPost.PATH, 'post');
  if (params) {
    rb.body(params.body, 'application/json');
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<SearchRankPresentationResponsesLoginResponse>;
    })
  );
}

apiTokenPost.PATH = '/api/token';
