/* tslint:disable */
/* eslint-disable */
/* Code generated by ng-openapi-gen DO NOT EDIT. */

import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiTokenPost } from '../fn/bearer-token/api-token-post';
import { ApiTokenPost$Params } from '../fn/bearer-token/api-token-post';
import { LoginResponse as SearchRankPresentationResponsesLoginResponse } from '../models/SearchRank/Presentation/Responses/login-response';

@Injectable({ providedIn: 'root' })
export class BearerTokenService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiTokenPost()` */
  static readonly ApiTokenPostPath = '/api/token';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiTokenPost()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  apiTokenPost$Response(params: ApiTokenPost$Params, context?: HttpContext): Observable<StrictHttpResponse<SearchRankPresentationResponsesLoginResponse>> {
    return apiTokenPost(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiTokenPost$Response()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  apiTokenPost(params: ApiTokenPost$Params, context?: HttpContext): Observable<SearchRankPresentationResponsesLoginResponse> {
    return this.apiTokenPost$Response(params, context).pipe(
      map((r: StrictHttpResponse<SearchRankPresentationResponsesLoginResponse>): SearchRankPresentationResponsesLoginResponse => r.body)
    );
  }

}
